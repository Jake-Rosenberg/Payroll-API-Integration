using Refit;
using UKG_Integration_Application.Model;

namespace UKG_Integration_Application.Services.Interfaces
{
    public interface IUkgAccessToken
    {
        // Auth Token
        [Post("/authentication/access_token")]
        Task<TokenDTO> PostAccessTokenAsync(
            [Header("content-type")] string contentType,
            [Header("appkey")] string appKey,
            [Body] FormUrlEncodedContent request);
    }
}
