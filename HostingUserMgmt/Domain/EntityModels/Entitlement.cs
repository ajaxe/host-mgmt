using System.Collections.Generic;

namespace HostingUserMgmt.Domain.EntityModels
{
    public class Entitlement: EntityBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RoleEntitlement> Roles { get; set; }
    }
}