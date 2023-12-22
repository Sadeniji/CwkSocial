using System.Security.Claims;

namespace CwkSocial.Api.Extensions;

public static class HttpContextExtension
{
    public static Guid GetUserProfileIdClaimValue(this HttpContext context) => GetClaimValueInGuid("UserProfileId", context);

    public static Guid GetIdentityIdClaimValue(this HttpContext context) => GetClaimValueInGuid("IdentityId", context);
    private static Guid GetClaimValueInGuid(string key, HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        return Guid.Parse(identity?.FindFirst(key)?.Value);
    }

}