using HostingUserMgmt.Domain.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HostingUserMgmt.Repository.EntityConfiguration
{
    public class ApiCredentialConfiguration : IEntityTypeConfiguration<ApiCredential>
    {
        public void Configure(EntityTypeBuilder<ApiCredential> builder)
        {
            builder.HasIndex(p => p.Username)
                .IsUnique();
        }
    }
}