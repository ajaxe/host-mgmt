namespace HostingUserMgmt.Helpers.Configuration
{
    public class AwsConfig
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string Region { get; set; }
        public string KmsMasterKeyId { get; set; }
        public string EncryptedDataKey { get; set; }
    }
}