using HostingUserMgmt.Domain.Constants;

namespace HostingUserMgmt.Domain.EntityModels
{
    public class ApiCredential: EntityBase
    {
        public int Id { get; set; }
        public CredentialType Type { get; set; }
        public string Username { get; set; }
        public string UserSecret { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}