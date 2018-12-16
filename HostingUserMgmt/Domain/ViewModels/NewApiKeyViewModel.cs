namespace HostingUserMgmt.Domain.ViewModels
{
    public class NewApiKeyViewModel
    {
        public string KeyName { get; set; }
        public string KeySecret { get;set; }
        public string HashedKeySecret { get; set; }
    }
}