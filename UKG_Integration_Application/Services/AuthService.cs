using Domain.Entities.UKG;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UKG_Integration_Application.Services
{
    public interface IAuthService
    {
        (string contentType, string appKey, OAuthRequest oAuthRequest) UKGAuthToken();
    }

    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(ILogger<AuthService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public (string contentType, string appKey, OAuthRequest oAuthRequest) UKGAuthToken()
        {
            _logger.LogInformation("Initiating access to auth info from AppSettings.");
            string? contentType = _configuration[""];
            string? appKey = _configuration[""];
            OAuthRequest? oAuthRequest = _configuration.GetSection("").Get<OAuthRequest>();

            _logger.LogInformation(
                contentType,
                appKey,
                oAuthRequest.Client_Id,
                oAuthRequest.Client_Secret,
                oAuthRequest.Grant_Type,
                oAuthRequest.Auth_Chain);

            _logger.LogInformation("Authentication process completed.");
            return (contentType, appKey, oAuthRequest);
        }
    }
}
