using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using HostingUserMgmt.Domain.EntityModels;

namespace HostingUserMgmt.AppServices.Mapping
{
    public class ViewToRepoModelProfile: Profile
    {
        public ViewToRepoModelProfile()
        {
            CreateMap<IEnumerable<Claim>, User>()
            .ForMember(dest => dest.ExternalId,
                opts => MapFromClaimType(opts, ClaimTypes.NameIdentifier))
            .ForMember(dest => dest.Name,
                opts => MapFromClaimType(opts, ClaimTypes.Name))
            .ForMember(dest => dest.EmailAddress,
                opts => MapFromClaimType(opts, ClaimTypes.Email));
        }
        private void MapFromClaimType(IMemberConfigurationExpression<IEnumerable<Claim>, User, string> opts,
            string claimType)
        {
            opts.MapFrom(src => src.First(c => c.Type == claimType).Value);
        }
    }
}