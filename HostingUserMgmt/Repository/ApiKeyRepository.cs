using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HostingUserMgmt.Repository
{
    public class ApiKeyRepository: IApiKeyRepository
    {
        private readonly HostingManagementDbContext context;
        public ApiKeyRepository(HostingManagementDbContext context)
        {
            this.context = context;
        }

        public async Task AddAsync(ApiCredential credential)
        {
            await context.ApiCredentials.AddAsync(credential);
            await context.SaveChangesAsync();
        }

        public async Task<IList<ApiCredential>> GetPartialApiKeys(int userId)
        {
            return await context.ApiCredentials
            .Where(creds => creds.UserId == userId)
            .Select(creds => new ApiCredential
            {
                Id = creds.Id,
                CreatedAtUtc = creds.CreatedAtUtc
            })
            .ToListAsync();
        }
    }
}