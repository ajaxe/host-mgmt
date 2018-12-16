using System.Threading.Tasks;

namespace HostingUserMgmt.AppServices.Abstractions
{
    public interface IEncryptionService
    {
        Task<string> GetEncryptedRandomStringAsync();

        string DecryptString(string cipherText, string salt);
    }
}