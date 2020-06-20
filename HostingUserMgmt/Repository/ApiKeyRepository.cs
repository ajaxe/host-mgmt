using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HostingUserMgmt.Repository
{
    public class ApiKeyRepository : IApiKeyRepository
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

        public async Task<ApiCredential> GetApiKeyByIdAsync(int keyId)
        {
            return await context.ApiCredentials.FindAsync(keyId);
        }

        public async Task<IList<ApiCredential>> GetApiKeysByUserIdAsync(int userId)
        {
            return await context.ApiCredentials
            .Where(creds => creds.UserId == userId)
            .Select(creds => new ApiCredential
            {
                Id = creds.Id,
                Username = creds.Username,
                CreatedAtUtc = creds.CreatedAtUtc
            })
            .ToListAsync();
        }

        public async Task<bool> IsCredentialUsernameAvailableAsync(string keyName)
        {
            var present = await context.ApiCredentials.AnyAsync(ap => ap.Username == keyName);
            return !present;
        }

        public async Task DeleteApiCredentialAsync(ApiCredential apiCreds)
        {
            if (apiCreds == null)
            {
                throw new ArgumentNullException(nameof(apiCreds));
            }
            context.ApiCredentials.Remove(apiCreds);
            await context.SaveChangesAsync();
        }

        public async Task<ApiCredential> GetApiKeyByUsernameAsync(string apiCredentialUsername)
        {
            if (string.IsNullOrWhiteSpace(apiCredentialUsername))
            {
                throw new ArgumentException("Null or empty", nameof(apiCredentialUsername));
            }
            return await context.ApiCredentials.Where(creds => creds.Username == apiCredentialUsername).FirstAsync();
        }
    }
}