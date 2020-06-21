using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace HostingUserMgmt.Helpers.Authentication
{
    public class GoogleClaimsProcessor : ClaimAction
    {
        private string claimType;
        public GoogleClaimsProcessor(string claimType, string valueType = ClaimValueTypes.String) : base(claimType, valueType)
        {
            this.claimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }
        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            var imageClaim = userData.GetProperty(claimType);
            identity.AddClaim(new Claim(claimType, imageClaim.GetString(), ValueType, issuer));
        }
    }
}