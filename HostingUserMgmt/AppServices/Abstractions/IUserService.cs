using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HostingUserMgmt.Domain.ViewModels;

namespace HostingUserMgmt.AppServices.Abstractions
{
    public interface IUserService
    {
        Task<UserProfileViewModel> GetUserProfile();
        Task AddOrUpdateUserOnLogin(IEnumerable<Claim> claims);
    }
}