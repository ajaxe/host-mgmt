using System.Threading.Tasks;
using HostingUserMgmt.Domain.ViewModels;

namespace HostingUserMgmt.AppServices.Abstractions
{
    public interface IEncryptionService
    {
        Task<string> GetEncryptedRandomStringAsync();

        Task<string> DecryptString(string encryptedString);
        Task<NewApiKeyViewModel> GenerateApiKey();

        bool VerifySecret(string plainText, string encryptedString);
    }
}