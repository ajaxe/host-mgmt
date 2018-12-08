using System.Collections.Generic;

namespace HostingUserMgmt.Domain.EntityModels
{
    public class User: EntityBase
    {
        public int Id { get; set; }
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public List<ApiCredential> Credentials { get; set; }
    }
}