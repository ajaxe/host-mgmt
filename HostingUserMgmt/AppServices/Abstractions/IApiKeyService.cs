using System.Collections.Generic;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.ViewModels;

namespace HostingUserMgmt.AppServices.Abstractions
{
    public interface IApiKeyService
    {
        Task<IList<ApiKeyDisplayViewModel>> GetApiKeysForDisplayAsync();
        Task<ApiKeyDisplayViewModel> CreateApiKey();
    }
}