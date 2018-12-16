using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Helpers.Configuration;
using Microsoft.Extensions.Options;

namespace HostingUserMgmt.AppServices
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IAmazonKeyManagementService kmsService;
        private string kmsMasterKeyId;
        private string encryptedDataKey;
        public EncryptionService(IAmazonKeyManagementService kmsService,
            IOptions<AwsConfig> options)
        {
            this.kmsService = kmsService ?? throw new ArgumentNullException(nameof(kmsService));
            this.kmsMasterKeyId = options?.Value?.KmsMasterKeyId ?? throw new ArgumentNullException("KmsMasterKeyId");
            this.encryptedDataKey = options?.Value?.EncryptedDataKey ?? throw new ArgumentNullException("EncryptedDataKey");
        }
        public string DecryptString(string cipherText, string salt)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> GetEncryptedRandomStringAsync()
        {
            byte[] salt = null;
            var secret = new byte[20];
            using(var randomGen = RandomNumberGenerator.Create())
            {
                randomGen.GetBytes(secret);
            }
            using(var aesEncryption = new AesManaged())
            {
                aesEncryption.GenerateIV();
                salt = aesEncryption.IV;
                using (var buffer = new MemoryStream())
                using (var transform = aesEncryption.CreateEncryptor(await GetDecryptedDataKey(), aesEncryption.IV))
                using (var stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(secret);
                    }

                    return Convert.ToBase64String(salt.Concat(buffer.ToArray()).ToArray());
                }
            }
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
    }
}