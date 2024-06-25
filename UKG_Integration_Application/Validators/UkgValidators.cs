using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using UKG_Integration_Application.API_UKG;
using UKG_Integration_Application.Model;

namespace UKG_Integration_Application.Validators
{
    public class TokenValidator : AbstractValidator<TokenDTO>
    {
        public TokenValidator()
        {
            RuleFor(response => response.access_token).NotEmpty();
            RuleFor(response => response.expires_in).NotNull().GreaterThan(50);
        }
    }

    public class HyperfindValidator : AbstractValidator<HyperfindDTO>
    {
        public HyperfindValidator()
        {
            RuleFor(response => response.count).GreaterThanOrEqualTo(1000).WithSeverity(Severity.Warning);
        }
    }
}
