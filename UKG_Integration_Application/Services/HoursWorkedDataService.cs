using AutoMapper;
using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Refit;
using UKG_Integration_Application.API_UKG.DTOs;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface IHoursWorkedDataService
    {
        Task<List<HoursWorked>> GetHoursWorkedData(List<int> ids);
    }

    public class HoursWorkedDataService(
        IUkgApi ukgApi,
        ILogger<HoursWorkedDataService> logger,
        IBatchingService batchingService,
        IBaseRepository<HoursWorked> baseRepo,
        IMapper mapper) : IHoursWorkedDataService
    {
        private readonly IUkgApi _ukgApi = ukgApi;
        private readonly ILogger<HoursWorkedDataService> _logger = logger;
        private readonly IBatchingService _batchingService = batchingService;
        private readonly IBaseRepository<HoursWorked> _baseRepo = baseRepo;
        private readonly IMapper _mapper = mapper;

        // TODO: see if you can refactor this class so that it's just a template for all queries
        private async Task<List<HoursWorkedIdHourPair>> HoursWorkedDataCall(List<int> ids)
        {

            _logger.LogInformation("Hours Worked Service call initiated.");

            var employeeIds = ids;
            string firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy-MM-dd");
            string yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            try
            {
                var response = await _ukgApi.PostHoursWorkedData(
                    new Body(
                        // SELECT
                        [
                            new ("PersonId", "EMP_COMMON_ID"),
                            new ("Hours", "TIMECARD_TRANS_ACTUAL_HOURS"),
                            new ("TransactionType", "TIMECARD_TRANS_TRANSACTION_TYPE")
                        ],
                        // FROM
                        new From(
                            new EmployeeSet(
                                new DateRange(
                                    null,
                                    // Start Date
                                    firstDayOfYear,
                                    // End Date
                                    yesterday
                                ),
                                new Employees(
                                    employeeIds
                                )
                            ),
                            "EMP"
                        ),
                        // OPTIONS
                        new Options(
                            true
                        ),
                        // WHERE
                        [
                            new ("TransactionType",
                                 "TIMECARD_TRANS_TRANSACTION_TYPE",
                                 "EQUAL_TO",
                                 ["Worked Shift Segment"])
                        ],
                        // GROUP BY
                        [
                            new("PersonId",
                                "EMP_COMMON_ID")
                        ]
                    )
                );

                List<HoursWorkedIdHourPair> details = response.Data.Children;

                _logger.LogInformation("Hours Worked Data call successful.");
                return details;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Hours Worked Data call error: {message}, CONTENT:{content}", ex.Message, ex.Content);
                throw ex;
            }
        }

        public async Task<List<HoursWorked>> GetHoursWorkedData(List<int> ids)
        {
            _logger.LogInformation("Hours Worked data FUNCTION initiated.");

            var allMappedData = new List<HoursWorked>();

            var batchSize = 400;
            int offset = 0;

            try
            {
                var pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);

                while (pagedIds.Count != 0)
                {
                    _logger.LogInformation("Batch offset = {offset}", offset);

                    // API Call
                    var hoursWorkedDataBatch = await HoursWorkedDataCall(pagedIds);
                    offset += batchSize;

                    // Map Results
                    var mappedData = _mapper.Map<IEnumerable<HoursWorked>>(hoursWorkedDataBatch);

                    // Add to list and eval next batch
                    allMappedData.AddRange(mappedData);
                    pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);
                }

                // Save to DB
                //await _baseRepo.AddRangeAsync(allMappedData, CancellationToken.None);

                _logger.LogInformation("Hours Worked data returned successfully. Result count: {count}", allMappedData.Count);
                return allMappedData;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hours Worked data FUNCTION exception: {message}", ex.Message);
                throw ex;
            }
        }

    }
}
