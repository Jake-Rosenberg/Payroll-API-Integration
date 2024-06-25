using Domain.Entities.UKG;
using Microsoft.Extensions.Logging;
using UKG_Integration_Application.Database.BaseRepo;
using UKG_Integration_Application.Database.DAL;

namespace UKG_Integration_Application.Services.PersistenceServices
{
    public interface IPaycodeEditService
    {
        Task AddPaycodeEditsAsync(List<PaycodeEdit> paycodeEdits);
    }

    public class PaycodeEditService(
        IBaseRepository<PaycodeEdit> paycodeEditRepo,
        IPaycodeEditDAL paycodeEditDAL,
        ILogger<PaycodeEditService> logger) : IPaycodeEditService
    {
        private readonly IBaseRepository<PaycodeEdit> _paycodeEditRepo = paycodeEditRepo;
        private readonly IPaycodeEditDAL _paycodeEditDAL = paycodeEditDAL;
        private readonly ILogger<PaycodeEditService> _logger = logger;

        // TODO: refactor all of the PaycodeEditService base repo stuff into the DAL

        public async Task AddPaycodeEditsAsync(List<PaycodeEdit> paycodeEdits)
        {
            bool isPaycodeEditsEmpty = !_paycodeEditRepo.Any();

            var paycodeEditsToAdd = new List<PaycodeEdit>();

            if (isPaycodeEditsEmpty)
            {
                paycodeEditsToAdd = paycodeEdits;
            }
            else
            {
                var maxPaycodeEditIds = await _paycodeEditDAL.GetMaxDatePaycodeEditIds();
                var paycodeEditIds = paycodeEdits.Select(p => p.Id);
                var peIdsToDelete = from maxId in maxPaycodeEditIds
                                    join wsId in paycodeEditIds on maxId equals wsId
                                    select maxId;

                paycodeEditsToAdd = paycodeEdits.Where(pe => !peIdsToDelete.Contains(pe.Id)).ToList();
            }

            try
            {
                await _paycodeEditRepo.AddRangeAsync(paycodeEditsToAdd, CancellationToken.None);
                await _paycodeEditRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            
        }
    }

}
