using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HostingUserMgmt.Domain.ViewModels
{
    public class UserProfile
    {
        public UserProfile(IEnumerable<Claim> claims)
        {
            if(claims == null || !claims.Any())
            {
                throw new ArgumentException("Null or empty claims", nameof(claims));
            }
            Name = claims.First(c => c.Type == ClaimTypes.Name).Value;
            EmailAddress = claims.First(c => c.Type == ClaimTypes.Email).Value;
            ImageUrl = claims.First(c => c.Type == "image.url").Value;
        }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }
    }
}