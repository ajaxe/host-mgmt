using AutoMapper;
using HostingUserMgmt.Domain.EntityModels;
using HostingUserMgmt.Domain.ViewModels;

namespace HostingUserMgmt.AppServices.Mapping
{
    public class RepoToViewModelProfile: Profile
    {
        public RepoToViewModelProfile()
        {
            CreateMap<User, UserProfileViewModel>()
                .ForMember(dest => dest.UserId, opts => opts.MapFrom(src => src.Id));
            CreateMap<ApiCredential, ApiKeyDisplayViewModel>()
                .ForMember(dest => dest.ApiKeyId, opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedAtUtc, opts => opts.MapFrom(src => src.CreatedAtUtc.Value.ToString("r")));
        }
    }
}