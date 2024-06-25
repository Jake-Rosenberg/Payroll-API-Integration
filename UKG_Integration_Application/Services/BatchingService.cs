using Microsoft.Extensions.Logging;

namespace UKG_Integration_Application.Services
{
    public interface IBatchingService
    {
        List<int>? GetPaginatedIds(List<int> ids, int offset, int batchsize);
    }

    public class BatchingService : IBatchingService
    {
        private readonly ILogger<BatchingService> _logger;

        public BatchingService(ILogger<BatchingService> logger)
        {
            _logger = logger;
        }

        public List<int>? GetPaginatedIds(List<int> ids, int offset, int batchsize)
        {
            try
            {
                var pagedIds = Paginate(offset, batchsize, ids);
                return pagedIds;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ID pagination exception: {message}", ex.Message);
                throw ex;
            }
        }

        public static List<int>? Paginate(int offset, int batchsize, List<int> ids)
        {
            var pagedIds = ids
                    .Skip(offset)
                    .Take(batchsize)
                    .ToList();

            return pagedIds;
        }
    }
}
