using System.Security.Claims;

namespace Shared
{
    public static class AuthClaimsExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub") ?? throw new InvalidOperationException("UserId claim not found in token.");
            return Guid.TryParse(claim, out var userId) ? userId : throw new InvalidOperationException("UserId claim is not a valid Guid.");
        }
    }
}
