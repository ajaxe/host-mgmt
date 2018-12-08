namespace HostingUserMgmt.Domain.EntityModels
{
    public class RoleEntitlement
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int EntitlementId { get; set; }
        public Entitlement Entitlement { get; set; }
    }
}