using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HostingUserMgmt.Domain.ViewModels
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string ImageUrl { get; set; }
    }
}