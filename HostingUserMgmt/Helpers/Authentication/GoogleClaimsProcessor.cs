using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Security.Claims;

namespace HostingUserMgmt.Helpers.Authentication
{
    public class GoogleClaimsProcessor : ClaimAction
    {
        private string claimType;
        public GoogleClaimsProcessor(string claimType, string valueType = ClaimValueTypes.String) : base(claimType, valueType)
        {
            this.claimType = claimType ?? throw new ArgumentNullException(nameof(claimType));
        }
        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        {
            var imageClaim = userData.SelectTokens(claimType);
            foreach(var c  in imageClaim)
            {
                if(c.Path.Equals(claimType)) {
                    identity.AddClaim(new Claim(claimType, c.Value<string>(), ValueType, issuer));
                }
            }
        }
    }
}