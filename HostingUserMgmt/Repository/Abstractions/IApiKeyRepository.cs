using System.Collections.Generic;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.EntityModels;

namespace HostingUserMgmt.Repository.Abstractions
{
    public interface IApiKeyRepository
    {
        Task<IList<ApiCredential>> GetPartialApiKeys(int userId);
        Task AddAsync(ApiCredential credential);
    }
}