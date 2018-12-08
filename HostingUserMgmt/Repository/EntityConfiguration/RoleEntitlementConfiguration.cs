using HostingUserMgmt.Domain.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostingUserMgmt.Repository.EntityConfiguration
{
    public class RoleEntitlementConfiguration : IEntityTypeConfiguration<RoleEntitlement>
    {
        public void Configure(EntityTypeBuilder<RoleEntitlement> builder)
        {
            builder.HasKey(re => new { re.RoleId, re.EntitlementId });
            builder.HasOne(re => re.Role);
            builder.HasOne(re => re.Entitlement);
        }
    }
}