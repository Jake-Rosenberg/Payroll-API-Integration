using AutoMapper;
using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Refit;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface ID_LateDataService
    {
        Task<List<D_LatePaycodeEdits>> GetLatePunchData(List<int> ids);
    }

    public class D_LateDataService(
        IUkgApi ukgApi,
        ILogger<D_LateDataService> logger,
        IBatchingService batchingService,
        IBaseRepository<D_LatePaycodeEdits> baseRepo,
        IMapper mapper) : ID_LateDataService
    {
        private readonly IUkgApi _ukgApi = ukgApi;
        private readonly ILogger<D_LateDataService> _logger = logger;
        private readonly IBatchingService _batchingService = batchingService;
        private readonly IBaseRepository<D_LatePaycodeEdits> _baseRepo = baseRepo;
        private readonly IMapper _mapper = mapper;

        // TODO: see if you can refactor this class so that it's just a template for all queries
        private async Task<List<PunchObject>> LatePunchDataCall(List<int> ids)
        {

            _logger.LogInformation("Late Punch Service call initiated.");

            var employeeIds = ids;

            try
            {
                var response = await _ukgApi.PostLatePunchData(
                    new Body(
                        // SELECT
                        [
                            new ("ShiftStart", "TIMECARD_TRANS_START_DATETIME"),
                            new ("PayCode", "CORE_PAYCODE"),
                            new ("PunchIn", "TIMECARD_TRANS_END_DATETIME"),
                            new ("HoursLate", "TIMECARD_TRANS_ACTUAL_HOURS"),
                            new ("OrgPath", "EMP_COMMON_PRIMARY_ORG")
                        ],
                        // FROM
                        new From(
                            new EmployeeSet(
                                new DateRange(
                                    new SymbolicPeriod(
                                        8 // yesterday symbolic period = 8
                                    ),
                                    null, // 2023-01-08 - beginning of UKG data
                                    null  // yesterday's date
                                ),
                                new Employees(
                                    employeeIds
                                )
                            ),
                            "EMP"
                        ),
                        // OPTIONS
                        new Options(
                            false
                        ),
                        // WHERE
                        [
                            new ("PayCode",
                                 "CORE_PAYCODE",
                                 "EQUAL_TO",
                                 ["D_Late"]),
                            new ("HoursLate",
                                 "TIMECARD_TRANS_ACTUAL_HOURS",
                                 "GREATER_THAN_OR_EQUAL_TO",
                                 ["00:30"])
                        ],
                        // GROUP BY
                        null
                    )
                );

                _logger.LogInformation("Record count from Late Punch Data call: {count}", response.metadata.totalElements.ToString());

                List<PunchObject> details = response.data.children;

                _logger.LogInformation("Late Punch Data call successful.");
                return details;
            }
            catch( ApiException ex )
            {
                _logger.LogError(ex, "Late Punch Data call error: {message}, CONTENT:{content}", ex.Message, ex.Content);
                throw ex;
            }
        }

        public async Task<List<D_LatePaycodeEdits>> GetLatePunchData(List<int> ids)
        {
            _logger.LogInformation("Late Punch data FUNCTION initiated.");

            var allMappedData = new List<D_LatePaycodeEdits>();

            var batchSize = 400;
            int offset = 0;

            try
            {
                var pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);

                while (pagedIds.Count != 0)
                {
                    _logger.LogInformation("Batch offset = {offset}", offset);

                    // API Call
                    var employeeDataBatch = await LatePunchDataCall(pagedIds);
                    offset += batchSize;

                    // Map Results
                    var mappedData = _mapper.Map<IEnumerable<D_LatePaycodeEdits>>(employeeDataBatch);

                    // Add to list and eval next batch
                    allMappedData.AddRange(mappedData);
                    pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);
                }

                _logger.LogInformation("Late Punch data returned successfully. Result count: {count}", allMappedData.Count);
                return allMappedData;

            }
            catch ( Exception ex )
            {
                _logger.LogError(ex, "Late Punch data FUNCTION exception: {message}", ex.Message);
                throw ex;
            }
        }
    }
}
