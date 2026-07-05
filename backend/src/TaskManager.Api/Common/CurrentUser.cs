using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TaskManager.Api.Common;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid Id
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return Guid.TryParse(value, out var id)
                ? id
                : throw new UnauthorizedException("The request is not associated with an authenticated user.");
        }
    }
}
