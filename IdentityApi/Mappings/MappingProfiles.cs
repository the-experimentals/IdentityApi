using AutoMapper;
using IdentityApi.ResponseModels;

namespace IdentityApi.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {            
            CreateMap<PolicyApi.Protos.TokenResponse, TokenResponse>()
                .ForMember(dest => dest.ACCESS, opt => opt.MapFrom(src => src.ACCESS));

            //CreateMap<Toggle2fa, Protos.toggle2faRequest>()
            //    .ForMember(dest => dest.ENABLED, opt => opt.MapFrom(src => src.ENABLED));
        }
    }
}
