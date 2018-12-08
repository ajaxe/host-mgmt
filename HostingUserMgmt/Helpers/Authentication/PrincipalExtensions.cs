using System.Linq;
using System.Security.Claims;

namespace HostingUserMgmt.Helpers.Authentication
{
    public static class PrincipalExtensions
    {
        public static string GetNameIdentifier(this ClaimsPrincipal principal)
        {
            return GetClaim(principal, ClaimTypes.NameIdentifier);
        }
        private static string GetClaim(ClaimsPrincipal principal, string claimType)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}