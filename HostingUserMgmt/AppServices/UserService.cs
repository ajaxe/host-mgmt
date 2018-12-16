using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using HostingUserMgmt.AppServices.Abstractions;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Domain.ViewModels;
using HostingUserMgmt.Helpers.Authentication;
using HostingUserMgmt.Repository.Abstractions;

namespace HostingUserMgmt.AppServices
{
    public class UserService: IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ClaimsPrincipal principal;
        private readonly IMapper mapper;
        public UserService(IUserRepository userRepository,
            ClaimsPrincipal principal,
            IMapper mapper)
        {
            this.userRepository = userRepository;
            this.principal = principal;
            this.mapper = mapper;
        }

        public async Task<int> AddOrUpdateUserOnLoginAsync(IEnumerable<Claim> claims)
        {
            var id = GetNameIdentifier(claims);
            var user = await userRepository.GetUserByExternalIdAsync(id);
            var newUser = user != null ? mapper.Map<IEnumerable<Claim>, User>(claims, user): mapper.Map<User>(claims);
            if(user == null)
            {
                await userRepository.AddUser(newUser);
            }
            else
            {
                await userRepository.UpdateUser();
            }
            return newUser.Id;
        }

        public async Task DeleteUserByExternalIdAsync(string externalId)
        {
            if(string.IsNullOrWhiteSpace(externalId))
            {
                return;
            }
            if(principal.GetNameIdentifier() != externalId)
            {
                throw new InvalidOperationException();
            }
            await userRepository.DeleteUserByExternalIdAsync(externalId);
        }

        public async Task<UserProfileViewModel> GetUserProfileAsync()
        {
            var id = GetNameIdentifier(principal.Claims);
            var user = await userRepository.GetUserByExternalIdAsync(id);
            if(user == null)
            {
                throw new InvalidOperationException($"No user exists with external id: {id}");
            }
            var vm = mapper.Map<UserProfileViewModel>(user);
            vm.ImageUrl = principal.Claims
                .First(c => c.Type == AppClaimTypes.GoogleImageUrl).Value;
            return vm;
        }
        private string GetNameIdentifier(IEnumerable<Claim> claims)
        {
            if(claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
            return claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}