using Domain.Entities.UKG;
using System.Linq.Expressions;
using UKG_Integration_Application.Database.BaseRepo;

namespace UKG_Integration_Application.Database.DAL
{
    public interface IWorkedShiftDAL
    {
        List<int> GetMaxDateWorkedShiftIds();
    }

    public class WorkedShiftDAL(IBaseRepository<WorkedShift> baseRepository) : IWorkedShiftDAL
    {
        private readonly IBaseRepository<WorkedShift> _workedShiftRepo = baseRepository;

        public List<int> GetMaxDateWorkedShiftIds()
        {
            var dateTime = DateTime.Today.AddDays(-4);

            //var workedShifts = await _workedShiftRepo.GetAllAsync();
            //var maxDate = workedShifts.Select(e => e.PunchIn.Date).Max();

            Expression<Func<WorkedShift, bool>> condition = e => e.PunchIn >= dateTime; //.Equals(maxDate);

            var selectedShifts = _workedShiftRepo.FindByCondition(condition);

            var ids = selectedShifts.Select(ws => ws.Id).ToList();

            return ids;
        }
    }
}
