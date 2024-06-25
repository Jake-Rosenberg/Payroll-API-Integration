using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UKG_Integration_Application.Model;

namespace UKG_Integration_Application.Services
{
    public class AuthorizationHandler(IAuthService authService,
                                      IAccessTokenService accessTokenService,
                                      ILogger<AuthorizationHandler> logger,
                                      IConfiguration configuration,
                                      IMemoryCache cache) : DelegatingHandler
    {
        private readonly IAuthService _authService = authService;
        private readonly IAccessTokenService _accessTokenService = accessTokenService;
        private readonly ILogger<AuthorizationHandler> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMemoryCache _cache = cache;
        private const string AccessTokenCacheKey = "AccessToken";

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Authorization Handler initiated.");

            try
            {
                // Check if access token exists in cache
                if (!_cache.TryGetValue(AccessTokenCacheKey, out TokenDTO? accessToken) || string.IsNullOrEmpty(accessToken.access_token))
                {
                    // If not found or expired, obtain a new access token
                    var accessTokenRequest = _authService.UKGAuthToken();
                    accessToken = await _accessTokenService.GetAccessToken(accessTokenRequest.contentType,
                                                                           accessTokenRequest.appKey,
                                                                           accessTokenRequest.oAuthRequest);
                    // Cache the access token with expiration time
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(accessToken.expires_in));

                    _cache.Set(AccessTokenCacheKey, accessToken, cacheEntryOptions);
                }

                request.Headers.Add("Authorization", accessToken.access_token);
                request.Headers.Add("appkey", _configuration[""]);

                _logger.LogInformation("Authorization Handler completed.");
                return await base.SendAsync(request, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Authorization Handler Error : {message}", ex.Message);
                throw ex;
            }
        }
    }
}
