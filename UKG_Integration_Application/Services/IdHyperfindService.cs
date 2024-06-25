using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface IIdHyperfindService
    {
        Task<List<int>> GetEmployeeIds();
    }

    public class IdHyperfindService : IIdHyperfindService
    {
        private readonly IUkgApi _ukgApi;
        private readonly ILogger<IdHyperfindService> _logger;
        private readonly IValidator<HyperfindDTO> _validator;
        private readonly IMemoryCache _memoryCache;

        public IdHyperfindService(IUkgApi ukgApi, ILogger<IdHyperfindService> logger, IValidator<HyperfindDTO> validator, IMemoryCache memoryCache)
        {
            _ukgApi = ukgApi;
            _logger = logger;
            _validator = validator;
            _memoryCache = memoryCache;
        }


        public async Task<List<int>> GetEmployeeIds()
        {

            _logger.LogInformation("HyperFind Execution initiated.");

            try
            {
                // Response
                var response = await _ukgApi.PostExecuteHyperfind(
                    new ExecuteHyperfindBody(
                        new DateRange(
                            new SymbolicPeriod(
                                1
                            ),
                            null,
                            null
                        ),
                        new Hyperfind(
                            "All Home"
                        )
                    ));

                // Validation
                ValidationResult result = await _validator.ValidateAsync(response);

                // Checking to see if number of officers returned is greater than 1000
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Validation error: {error} SEVERITY : {severity}", error.ErrorMessage, error.Severity);
                    }
                }

                _logger.LogInformation("Ref count from Hyperfind Execution call: {count}", response.count.ToString());

                // Add count to memoryCache to use in EmployeeData validation rule
                _memoryCache.Set("EmpIdCount", response.count);

                List<HyperfindStaffRef> staffRefs = [.. response.result.refs];

                List<int> ids = staffRefs.Select(staffRef => staffRef.id).ToList();

                _logger.LogInformation("HyperFind Execution successful.");
                return ids;
            }
            catch( Exception ex )
            {
                _logger.LogError(ex, "HyperFind Execution error: {message}", ex.Message);
                throw ex;
            }

            
        }
    }
}
