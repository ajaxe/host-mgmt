using System;

namespace HostingUserMgmt.Domain.ViewModels
{
    public class ApiKeyDisplayViewModel
    {
        public int ApiKeyId { get; set; }
        public string ApiKeyName { get; set; }
        public string CreatedAtUtc { get; set; }
        public string Username { get; set; }
    }
}