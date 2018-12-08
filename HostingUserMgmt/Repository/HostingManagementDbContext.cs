using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Repository.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace HostingUserMgmt.Repository
{
    public class HostingManagementDbContext: DbContext
    {
        public HostingManagementDbContext(DbContextOptions<HostingManagementDbContext> options)
            :base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ApiCredential> ApiCredentials { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Entitlement> Entitlements { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleEntitlementConfiguration());
            builder.ApplyConfiguration(new ApiCredentialConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
        }
        public override int SaveChanges()
        {
            UpdateChangeTrackingProperties();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateChangeTrackingProperties();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void UpdateChangeTrackingProperties()
        {
            if(ChangeTracker.HasChanges())
            {
                var changes = ChangeTracker.Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
                foreach(var entry in changes)
                {
                    if(entry.State == EntityState.Added)
                    {
                        ((EntityBase)entry.Entity).CreatedAtUtc = DateTime.UtcNow;
                    }
                    else
                    {
                        ((EntityBase)entry.Entity).ModifiedAtUtc = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}