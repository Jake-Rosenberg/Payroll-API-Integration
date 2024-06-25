using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System.Linq;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;

namespace UKG_Integration_Application.Services.PersistenceServices
{
    public interface IWorkedShiftService
    {
        Task AddOrUpdateWorkedShiftsAsync(List<WorkedShift> workedShifts);
    }

    public class WorkedShiftService(
        IBaseRepository<WorkedShift> workedShiftRepo,
        IScheduleShiftDAL scheduleShiftDAL,
        IWorkedShiftDAL workedShiftDAL,
        ILogger<WorkedShiftService> logger) : IWorkedShiftService
    {

        private readonly IBaseRepository<WorkedShift> _workedShiftRepo = workedShiftRepo;
        private readonly IScheduleShiftDAL _scheduleShiftDAL = scheduleShiftDAL;
        private readonly IWorkedShiftDAL _workedShiftDAL = workedShiftDAL;
        private readonly ILogger<WorkedShiftService> _logger = logger;

        // TODO: refactor all of the WorkedShiftService base repo stuff into the DAL

        public async Task AddOrUpdateWorkedShiftsAsync(List<WorkedShift> workedShifts)
        {
            bool isWorkedShiftsEmpty = !_workedShiftRepo.Any();

            var workedShiftsToAdd = new List<WorkedShift>();

            List<int> maxScheduleShiftIds = await _scheduleShiftDAL.GetMaxDateScheduleShiftIds();

            if (isWorkedShiftsEmpty)
            {
                workedShiftsToAdd = workedShifts;
            }
            else
            {
                var wsIdsToUpdate = GetIdsToUpdate(workedShifts);

                var workedShiftsToUpdate = GetWorkedShiftsToUpdate(workedShifts, wsIdsToUpdate);

                await UpdateWorkedShiftsAsync(workedShiftsToUpdate);

                var workedShiftsNoUpdates = GetWorkedShiftsNoUpdates(workedShifts, wsIdsToUpdate);

                // filter out workedShifts containing a ScheduleShiftId with no corresponding ScheduleShift
                // TODO: determine whether this actually functions, is there a problem with the convert
                workedShiftsToAdd = GetWorkedShiftsToAdd(workedShiftsNoUpdates, maxScheduleShiftIds);
            }

            try
            {
                await _workedShiftRepo.AddRangeAsync(workedShiftsToAdd, CancellationToken.None);
                await _workedShiftRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private List<int> GetIdsToUpdate(List<WorkedShift> workedShifts)
        {
            var maxDbWorkedShiftIds = _workedShiftDAL.GetMaxDateWorkedShiftIds();
            var workedShiftIds = workedShifts.Select(ws => ws.Id);
            var wsIdsToUpdate = from maxId in maxDbWorkedShiftIds
                                join wsId in workedShiftIds on maxId equals wsId
                                select maxId;

            var ids = wsIdsToUpdate.ToList();

            return ids;
        }

        private static List<WorkedShift> GetWorkedShiftsToUpdate(List<WorkedShift> workedShifts, List<int> wsIdsToUpdate)
        {
            var workedShiftsToUpdate = workedShifts.Where(ws => wsIdsToUpdate.Contains(ws.Id));
            var wsList = workedShiftsToUpdate.ToList();
            return wsList;
        }

        private async Task UpdateWorkedShiftsAsync(List<WorkedShift> workedShiftsToUpdate)
        {
            foreach (WorkedShift apiShift in workedShiftsToUpdate)
            {
                var shiftFromDb = _workedShiftRepo.FindByCondition(ws => ws.Id.Equals(apiShift.Id)).FirstOrDefault();
                if (shiftFromDb != null)
                {
                    shiftFromDb.PunchOut = apiShift.PunchOut;
                    //shiftFromDb.Facility = apiShift.Facility;
                    //shiftFromDb.Department = apiShift.Department;
                    //shiftFromDb.Shift = apiShift.Shift;
                    shiftFromDb.Exception = apiShift.Exception;
                    _workedShiftRepo.Update(shiftFromDb);
                }
            }
            await _workedShiftRepo.SaveChangesAsync();
        }

        public List<WorkedShift> GetWorkedShiftsNoUpdates(List<WorkedShift> workedShifts, List<int> wsIdsToUpdate)
        {
            var workedShiftsNoUpdates = workedShifts
                .Where(ws => !wsIdsToUpdate.Contains(ws.Id))
                .ToList();

            var wsNoUpdates = new List<WorkedShift>(workedShiftsNoUpdates);

            foreach (var shift in wsNoUpdates)
            {
                _logger.Log(LogLevel.Information, $"WorkedShift - Id: {shift.Id}, ScheduledShiftId: {shift.ScheduledShiftId}, PunchIn: {shift.PunchIn}, PunchOut: {shift.PunchOut}");
            }
            _logger.LogInformation($"wsNoUpdates Count: {wsNoUpdates.Count}");

            return wsNoUpdates;
        }

        public List<WorkedShift> GetWorkedShiftsToAdd(List<WorkedShift> workedShiftsNoUpdates, List<int> maxScheduleShiftIds)
        {
            var wsWithCorrespondingScheduleShifts = workedShiftsNoUpdates
                .Where(ws => ws.ScheduledShiftId.HasValue && maxScheduleShiftIds.Contains(ws.ScheduledShiftId.Value))
                .ToList();

            var wsToAdd = new List<WorkedShift>(wsWithCorrespondingScheduleShifts);

            foreach (var shift in wsToAdd)
            {
                _logger.Log(LogLevel.Information, $"WorkedShiftsToAdd - Id: {shift.Id}, ScheduledShiftId: {shift.ScheduledShiftId}, PunchIn: {shift.PunchIn}, PunchOut: {shift.PunchOut}");
            }
            _logger.LogInformation($"WorkedShiftsToAdd Count: {wsToAdd.Count}");

            return wsToAdd;
        }
    }
}
