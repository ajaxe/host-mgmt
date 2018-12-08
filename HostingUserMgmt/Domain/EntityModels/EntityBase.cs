using System;

namespace HostingUserMgmt.Domain.EntityModels
{
    public abstract class EntityBase
    {
        public DateTime? CreatedAtUtc { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
    }
}