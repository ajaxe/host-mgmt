using System;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HostingUserMgmt.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly HostingManagementDbContext context;
        public UserRepository(HostingManagementDbContext context)
        {
            this.context = context;
        }
        public async Task AddUser(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
        public async Task UpdateUser()
        {
            await context.SaveChangesAsync();
        }

        public async Task<User> GetUserByExternalIdAsync(string externalId)
        {
            if(string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentNullException("External id is null or empty",
                    nameof(externalId));
            }
            return await context.Users.Where(u => externalId.Equals(u.ExternalId))
                .FirstOrDefaultAsync();
        }
    }
}