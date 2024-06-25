using Domain.Entities.UKG;
using System.Linq.Expressions;
using UKG_Integration_Application.Database.BaseRepo;

namespace UKG_Integration_Application.Database.DAL
{
    public interface IScheduleShiftDAL
    {
        Task<List<int>> GetMaxDateScheduleShiftIds();
    }

    public class ScheduleShiftDAL(IBaseRepository<ScheduleShift> baseRepository) : IScheduleShiftDAL
    {
        private readonly IBaseRepository<ScheduleShift> _scheduleShiftRepo = baseRepository;

        public async Task<List<int>> GetMaxDateScheduleShiftIds()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));

            var scheduleShifts = await _scheduleShiftRepo.GetAllAsync();
            //var maxDate = scheduleShifts.Select(e => e.EventDate).Max();

            Expression<Func<ScheduleShift, bool>> condition = e => e.EventDate >= date;//maxDate;

            var selectedScheduleShifts = _scheduleShiftRepo.FindByCondition(condition);

            var ids = selectedScheduleShifts.Select(ws => ws.Id).ToList();

            return ids;
        }
    }
}