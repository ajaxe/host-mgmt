using System.Collections.Generic;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.ViewModels;

namespace HostingUserMgmt.AppServices.Abstractions
{
    public interface IApiKeyService
    {
        Task<IList<ApiKeyDisplayViewModel>> GetApiKeysForDisplayAsync();
        Task<NewApiKeyViewModel> CreateApiKey();
        Task<ApiKeyViewModel> GetApiKeyByIdAsync(int keyId);
    }
}