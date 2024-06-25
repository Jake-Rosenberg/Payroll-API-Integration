using Domain.Entities.UKG;
using System.Linq.Expressions;
using UKG_Integration_Application.Database.BaseRepo;

namespace UKG_Integration_Application.Database.DAL
{
    public interface IPaycodeEditDAL
    {
        Task<List<int>> GetMaxDatePaycodeEditIds();
    }

    public class PaycodeEditDAL(IBaseRepository<PaycodeEdit> baseRepository) : IPaycodeEditDAL
    {
        private readonly IBaseRepository<PaycodeEdit> _paycodeEditRepo = baseRepository;

        public async Task<List<int>> GetMaxDatePaycodeEditIds()
        {
            DateOnly date = DateOnly.FromDateTime(DateTime.Today.AddDays(-4));

            var workedShifts = await _paycodeEditRepo.GetAllAsync();
            //var maxDate = workedShifts.Select(e => e.ApplyDate).Max();

            Expression<Func<PaycodeEdit, bool>> condition = e => e.ApplyDate >= date;//maxDate;

            var selectedpaycodeEdits = _paycodeEditRepo.FindByCondition(condition);

            var ids = selectedpaycodeEdits.Select(ws => ws.Id).ToList();

            return ids;
        }
    }
}
