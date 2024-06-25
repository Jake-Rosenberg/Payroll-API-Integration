using FluentValidation;
using Microsoft.Extensions.Logging;

namespace UKG_Integration_Application.Model
{
    public class TokenDTO
    {
        public required string access_token { get; set; }
        //public string refresh_token { get; set; }
        //public string scope { get; set; }
        //public string id_token { get; set; }
        //public required string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
