using System;
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

        public async Task<NewApiKeyViewModel> CreateApiKey()
        {
            var userId = principal.GetUserId();
            var newApiKey = await encryptionService.GenerateApiKey();
            var newCredential = new ApiCredential()
            {
                UserId = userId,
                UserSecret = newApiKey.HashedKeySecret,
                Username = newApiKey.KeyName
            };
            await apiKeyRepository.AddAsync(newCredential);
            newApiKey.HashedKeySecret = null;
            return newApiKey;
        }

        public async Task<IList<ApiKeyDisplayViewModel>> GetApiKeysForDisplayAsync()
        {
            var userId = principal.GetUserId();
            var partialKeys = await apiKeyRepository.GetApiKeysByUserIdAsync(userId);
            return mapper.Map<IList<ApiKeyDisplayViewModel>>(partialKeys);
        }
        public async Task<ApiKeyViewModel> GetApiKeyByIdAsync(int keyId)
        {
            var apiKey = await apiKeyRepository.GetApiKeyByIdAsync(keyId);
            if(apiKey == null)
            {
                return null;
            }
            var mappedKey = mapper.Map<ApiKeyViewModel>(apiKey);

            mappedKey.Key = await encryptionService.DecryptString(mappedKey.Key);

            return mappedKey;
        }
        public async Task<ApiKeyDisplayViewModel> DeleteApiKeyByIdAsync(int keyId)
        {
            var apiKey = await apiKeyRepository.GetApiKeyByIdAsync(keyId);
            if(apiKey == null)
            {
                return null;
            }
            if(principal.GetUserId() != apiKey.UserId)
            {
                throw new InvalidOperationException($"Key Id: {keyId} does not belong to user: {apiKey.UserId}");
            }
            var mapped = mapper.Map<ApiKeyDisplayViewModel>(apiKey);
            await apiKeyRepository.DeleteApiCredentialAsync(apiKey);
            return mapped;
        }
    }
}