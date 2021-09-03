using AutoMapper;
using IdentityApi.Protos;
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

            CreateMap<RequestModels.EmailRequest, EmailRequest>()
                .ForMember(dest => dest.TO, opt => opt.MapFrom(src => src.TO))
                .ForMember(dest => dest.SUBJECT, opt => opt.MapFrom(src => src.SUBJECT))
                .ForMember(dest => dest.CONTENT, opt => opt.MapFrom(src => src.CONTENT))
                .ForMember(dest => dest.HTML, opt => opt.MapFrom(src => src.HTML));
        }
    }
}
