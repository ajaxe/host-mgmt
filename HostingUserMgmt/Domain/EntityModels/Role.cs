using System.Collections.Generic;

namespace HostingUserMgmt.Domain.EntityModels
{
    public class Role: EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RoleEntitlement> Entitlements { get; set; }
    }
}