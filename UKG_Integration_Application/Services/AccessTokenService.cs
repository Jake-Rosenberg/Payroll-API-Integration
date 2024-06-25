using Domain.Entities.UKG;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Refit;
using UKG_Integration_Application.Model;
using UKG_Integration_Application.Services.Interfaces;

namespace UKG_Integration_Application.Services
{
    public interface IAccessTokenService
    {
        Task<TokenDTO> GetAccessToken(string contentType, string appKey, OAuthRequest oAuthRequest);
    }

    public class AccessTokenService : IAccessTokenService
    {
      
        private readonly IUkgAccessToken _ugkAccessToken;
        private readonly ILogger<AccessTokenService> _logger;
        private readonly IValidator<TokenDTO> _validator;

        public AccessTokenService(ILogger<AccessTokenService> logger, IUkgAccessToken ukgAccessToken, IValidator<TokenDTO> validator)
        {
            _logger = logger;
            _ugkAccessToken = ukgAccessToken;
            _validator = validator;
        }

        public async Task<TokenDTO> GetAccessToken(string contentType, string appKey, OAuthRequest oAuthRequest)
        {
            try
            {
                // Convert OAuthRequest to form-urlencoded content
                var requestBody = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "username", oAuthRequest.Username },
                    { "password", oAuthRequest.Password },
                    { "client_id", oAuthRequest.Client_Id },
                    { "client_secret", oAuthRequest.Client_Secret },
                    { "grant_type", oAuthRequest.Grant_Type },
                    { "auth_chain", oAuthRequest.Auth_Chain }
                });

                // Response
                TokenDTO response = await _ugkAccessToken.PostAccessTokenAsync(contentType, appKey, requestBody);

                // Validation
                ValidationResult result = await _validator.ValidateAsync(response);

                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Validation error: {error}", error.ErrorMessage);
                    }
                    return null;
                }

                _logger.LogInformation("Access token validated and successfully returned");
                return response;
            }

            catch (ApiException ex)
            {
                _logger.LogError(ex, "Access token exception: {message}, CONTENT:{content}", ex.Message, ex.Content);
                throw ex;
            }

        }
    }
}
