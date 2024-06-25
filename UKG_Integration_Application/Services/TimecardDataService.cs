using AutoMapper;
using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Refit;
using UKG_Integration_Application.API_UKG.DTOs;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface ITimecardDataService
    {
        Task<(List<WorkedShift>, List<PaycodeEdit>, List<ScheduleShift>)> GetTimecardData(List<int> ids);
    }

    public class TimecardDataService(
        IUkgApi ukgApi,
        ILogger<TimecardDataService> logger,
        IBatchingService batchingService,
        IMapper mapper) : ITimecardDataService
    {
        private readonly IUkgApi _ukgApi = ukgApi;
        private readonly ILogger<TimecardDataService> _logger = logger;
        private readonly IBatchingService _batchingService = batchingService;
        private readonly IMapper _mapper = mapper;

        private async Task<(IEnumerable<WorkedShift>, IEnumerable<PaycodeEdit>, IEnumerable<ScheduleShift>)> TimecardDataCall(List<int> ids)
        {

            _logger.LogInformation("Timecard Service call initiated.");

            var employeeIds = ids;

            try
            {
                var response = await _ukgApi.PostTimecardData(
                    new TcBody(
                        // SELECT
                        [
                            "WORKED_SHIFTS",
                            "PAYCODE_EDITS",
                            "SCHEDULE_SHIFT"
                        ],
                        // WHERE
                        new TcWhere(
                            new DateRange(
                                new SymbolicPeriod(
                                    8 // 8 = yesterday
                                ),
                                null, // 2023-01-08 - beginning of UKG data
                                null  // yesterday's date
                            ),
                            new Employees(
                                employeeIds
                            )
                        ),
                        new TcOptions(false, true)
                    )
                );

                _logger.LogInformation(
                    "Record count of Worked Shifts from Timecard Data call: {count}",
                    response.SelectMany(tc => tc.workedShifts).ToList().Count.ToString());
                _logger.LogInformation(
                    "Record count of Paycode Edits from Timecard Data call: {count}",
                    response.SelectMany(tc => tc.payCodeEdits).ToList().Count.ToString());
                _logger.LogInformation(
                    "Record count of Schedule Shifts from Timecard Data call: {count}",
                    response.SelectMany(tc => tc.scheduleShifts).ToList().Count.ToString());

                IEnumerable<WorkedShiftDTO> rawWorkedShifts = response.SelectMany(tc => tc.workedShifts);
                IEnumerable<PaycodeEditDTO> rawPaycodeEdits = response.SelectMany(tc => tc.payCodeEdits);
                IEnumerable<ScheduleShiftDTO> rawScheduleShifts = response.SelectMany(tc => tc.scheduleShifts);

                _logger.LogInformation("Timecard Data call successful.");

                var mappedWorkedShifts = _mapper.Map<IEnumerable<WorkedShift>>(rawWorkedShifts);
                var mappedPaycodeEdits = _mapper.Map<IEnumerable<PaycodeEdit>>(rawPaycodeEdits).ToList();
                var mappedScheduleShifts = _mapper.Map<IEnumerable<ScheduleShift>>(rawScheduleShifts).ToList();


                return (mappedWorkedShifts, mappedPaycodeEdits, mappedScheduleShifts);
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Timecard Data call error: {message}, CONTENT:{content}", ex.Message, ex.Content);
                throw ex;
            }
        }

        // TODO: work on this section, split up the return item into three entities and save each to db

        public async Task<(List<WorkedShift>, List<PaycodeEdit>, List<ScheduleShift>)> GetTimecardData(List<int> ids)
        {
            _logger.LogInformation("Timecard data FUNCTION initiated.");

            var allMappedData = (new List<WorkedShift>(), new List<PaycodeEdit>(), new List<ScheduleShift>());

            var batchSize = 200;
            int offset = 0;

            try
            {
                var pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);

                while (pagedIds.Count != 0)
                {
                    _logger.LogInformation("Batch offset = {offset}", offset);

                    // API Call
                    var timecards = await TimecardDataCall(pagedIds);
                    offset += batchSize;

                    // Add to list and eval next batch
                    allMappedData.Item1.AddRange(timecards.Item1);
                    allMappedData.Item2.AddRange(timecards.Item2);
                    allMappedData.Item3.AddRange(timecards.Item3);

                    pagedIds = _batchingService.GetPaginatedIds(ids, offset, batchSize);
                }

                _logger.LogInformation("Timecard data returned successfully. Worked Shift count: {count}", allMappedData.Item1.Count.ToString());
                return allMappedData;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Timecard data FUNCTION exception: {message}", ex.Message);
                throw ex;
            }
        }
    }
}
