using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Domain.ViewModels;
using HostingUserMgmt.Helpers.Configuration;
using HostingUserMgmt.Repository.Abstractions;
using Microsoft.Extensions.Options;

namespace HostingUserMgmt.AppServices
{
    public class EncryptionService : IEncryptionService
    {
        private const int IvLength = 16;
        private readonly IApiKeyRepository apiKeyRepository;
        private readonly IAmazonKeyManagementService kmsService;
        private string kmsMasterKeyId;
        private string encryptedDataKey;
        public EncryptionService(IAmazonKeyManagementService kmsService,
            IApiKeyRepository apiKeyRepository,
            IOptions<AwsConfig> options)
        {
            this.apiKeyRepository = apiKeyRepository;
            this.kmsService = kmsService ?? throw new ArgumentNullException(nameof(kmsService));
            this.kmsMasterKeyId = options?.Value?.KmsMasterKeyId ?? throw new ArgumentNullException("KmsMasterKeyId");
            this.encryptedDataKey = options?.Value?.EncryptedDataKey ?? throw new ArgumentNullException("EncryptedDataKey");
        }
        public async Task<string> DecryptString(string encryptedString)
        {
            if(string.IsNullOrWhiteSpace(encryptedString))
            {
                throw new ArgumentNullException(nameof(encryptedString));
            }
            var allBytes = Convert.FromBase64String(encryptedString);
            using(var aesEncryption = new AesManaged())
            {
                aesEncryption.IV = allBytes.Take(IvLength).ToArray();
                using (var buffer = new MemoryStream( allBytes.Skip(IvLength).ToArray()))
                using (var transform = aesEncryption.CreateDecryptor(await GetDecryptedDataKey(), aesEncryption.IV))
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (var reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        public async Task<string> GetEncryptedRandomStringAsync()
        {
            using(var aesEncryption = new AesManaged())
            {
                aesEncryption.GenerateIV();
                using (var buffer = new MemoryStream())
                using (var transform = aesEncryption.CreateEncryptor(await GetDecryptedDataKey(), aesEncryption.IV))
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        await writer.WriteAsync(GetRandomSecret());
                    }
                    return Convert.ToBase64String(aesEncryption.IV.Concat(buffer.ToArray()).ToArray());
                }
            }
        }
        public async Task<NewApiKeyViewModel> GenerateApiKey()
        {
            var keyName = await GetUniqueKeyId();
            var salt = GetRandomSecret(8);
            var secret = GetRandomSecret();
            byte[] hashedResult = null;
            using(var prv = new SHA256CryptoServiceProvider())
            {
                hashedResult = prv.ComputeHash(ASCIIEncoding.UTF8.GetBytes($"{salt}{secret}"));
            }
            return new NewApiKeyViewModel
            {
                KeyName = keyName,
                KeySecret = secret,
                HashedKeySecret = $"{salt}|{Convert.ToBase64String(hashedResult)}"
            };
        }
        private async Task<byte[]> GetDecryptedDataKey()
        {
            var request = new DecryptRequest
            {
                CiphertextBlob = new MemoryStream(Convert.FromBase64String(encryptedDataKey))
            };
            var response = await kmsService.DecryptAsync(request);
            return response.Plaintext.ToArray();
        }
        private string GetRandomSecret(int length = 20)
        {
            var secretBytes = new byte[length];
            using(var randomGen = RandomNumberGenerator.Create())
            {
                randomGen.GetBytes(secretBytes);
                return secretBytes.Select(bb => bb.ToString("x2"))
                    .Aggregate(string.Empty, (val, acc) => $"{acc}{val}");
            }
        }
        private async Task<string> GetUniqueKeyId()
        {
            const int attempts = 4;
            string keyId = null;
            for(int i=0; i < 4; i++)
            {
                keyId = GetRandomSecret(4);
                if(await apiKeyRepository.IsCredentialUsernameAvailableAsync(keyId))
                {
                    return keyId;
                }
            }
            throw new SystemException($"Failed to create unique 'KeyId' in {attempts} attempts");
        }
    }
}