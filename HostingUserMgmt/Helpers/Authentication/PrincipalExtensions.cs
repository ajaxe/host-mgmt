using System;
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
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            if(int.TryParse(GetClaim(principal, AppClaimTypes.AppUserId), out int result))
            {
                return result;
            }
            throw new InvalidOperationException($"Missing user id claim: {AppClaimTypes.AppUserId}");
        }
        private static string GetClaim(ClaimsPrincipal principal, string claimType)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}