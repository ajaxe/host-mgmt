using System.Collections.Generic;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.EntityModels;

namespace HostingUserMgmt.Repository.Abstractions
{
    public interface IApiKeyRepository
    {
        Task<IList<ApiCredential>> GetApiKeysByUserIdAsync(int userId);
        Task AddAsync(ApiCredential credential);
        Task<ApiCredential> GetApiKeyByIdAsync(int keyId);
        Task<bool> IsCredentialUsernameAvailableAsync(string keyName);
    }
}