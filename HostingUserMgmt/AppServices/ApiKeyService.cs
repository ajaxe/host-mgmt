using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Domain.ViewModels;
using HostingUserMgmt.Helpers.Authentication;
using HostingUserMgmt.Repository.Abstractions;

namespace HostingUserMgmt.AppServices
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository apiKeyRepository;
        private readonly ClaimsPrincipal principal;
        private readonly IEncryptionService encryptionService;
        private readonly IMapper mapper;
        public ApiKeyService(IApiKeyRepository apiKeyRepository,
            ClaimsPrincipal principal,
            IEncryptionService encryptionService,
            IMapper mapper)
        {
            this.apiKeyRepository = apiKeyRepository;
            this.principal = principal;
            this.encryptionService = encryptionService;
            this.mapper = mapper;
        }

        public async Task<ApiKeyDisplayViewModel> CreateApiKey()
        {
            var userId = principal.GetUserId();
            var newCredential = new ApiCredential()
            {
                UserId = userId,
                UserSecret = await encryptionService.GetEncryptedRandomStringAsync()
            };
            await apiKeyRepository.AddAsync(newCredential);
            return mapper.Map<ApiKeyDisplayViewModel>(newCredential);
        }

        public async Task<IList<ApiKeyDisplayViewModel>> GetApiKeysForDisplayAsync()
        {
            var userId = principal.GetUserId();
            var partialKeys = await apiKeyRepository.GetPartialApiKeys(userId);
            return mapper.Map<IList<ApiKeyDisplayViewModel>>(partialKeys);
        }
        public Task<ApiKeyDisplayViewModel> GetApiKeyByIdAsync(int keyId)
        {
            return null;
        }
    }
}