using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;

namespace UKG_Integration_Application.Services.PersistenceServices
{
    public interface IScheduleShiftService
    {
        Task AddScheduleShiftsAsync(List<ScheduleShift> scheduleShifts);
    }

    public class ScheduleShiftService(
        IBaseRepository<ScheduleShift> baseRepository,
        IScheduleShiftDAL scheduleShiftDAL,
        IPersonIdService personIdService,
        ILogger<ScheduleShiftService> logger) : IScheduleShiftService
    {
        private readonly IBaseRepository<ScheduleShift> _scheduleShiftRepo = baseRepository;
        private readonly IScheduleShiftDAL _scheduleShiftDAL = scheduleShiftDAL;
        private readonly IPersonIdService _personIdService = personIdService;
        private readonly ILogger<ScheduleShiftService> _logger = logger;

        // TODO: refactor all of the ScheduleShiftService base repo stuff into the DAL

        public async Task AddScheduleShiftsAsync(List<ScheduleShift> scheduleShifts)
        {
            var maxScheduleShiftIds = await _scheduleShiftDAL.GetMaxDateScheduleShiftIds();
            var scheduleShiftIds = scheduleShifts.Select(s => s.Id);
            var sIdsToDelete = from maxId in maxScheduleShiftIds
                               join wsId in scheduleShiftIds on maxId equals wsId
                               select maxId;

            var allDbEmpIds = _personIdService.GetAllDbEmployeeIds();
            var ssPersonIds = scheduleShifts.Select(p => p.Id).ToList();

            List <ScheduleShift> shiftsToAdd = scheduleShifts.Where(s => !sIdsToDelete.Contains(s.Id)).ToList();

            var shiftsNoEmp = shiftsToAdd.Where(sta => !allDbEmpIds.Contains(sta.PersonId)).ToList();
            foreach (var shift in shiftsNoEmp)
            {
                _logger.Log(LogLevel.Information, $"shiftsNoEmp - Id: {shift.Id}, PersonId: {shift.PersonId}, EventDate: {shift.EventDate}");
            }
            _logger.LogInformation($"shiftsNoEmp Count: {shiftsNoEmp.Count}");

            try
            {
                await _scheduleShiftRepo.AddRangeAsync(shiftsToAdd, CancellationToken.None);
                await _scheduleShiftRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}
