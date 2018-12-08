using HostingUserMgmt.Domain.EntityModels;
using System.Threading.Tasks;

namespace HostingUserMgmt.Repository.Abstractions
{
    public interface IUserRepository
    {
        Task AddUser(User user);
        Task UpdateUser();
        Task<User> GetUserByExternalIdAsync(string externalId);
        Task DeleteUserByExternalIdAsync(string externalId);
    }
}