using AutoMapper;
using IdentityApi.RequestModels;
using PolicyApi.Protos;

namespace IdentityApi.Mappings;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<TokenResponse, ResponseModels.TokenResponse>()
            .ForMember(dest => dest.ACCESS, opt => opt.MapFrom(src => src.ACCESS));

        //CreateMap<Toggle2fa, Protos.toggle2faRequest>()
        //    .ForMember(dest => dest.ENABLED, opt => opt.MapFrom(src => src.ENABLED));

        CreateMap<EmailRequest, Protos.EmailRequest>()
            .ForMember(dest => dest.TO, opt => opt.MapFrom(src => src.TO))
            .ForMember(dest => dest.SUBJECT, opt => opt.MapFrom(src => src.SUBJECT))
            .ForMember(dest => dest.CONTENT, opt => opt.MapFrom(src => src.CONTENT))
            .ForMember(dest => dest.HTML, opt => opt.MapFrom(src => src.HTML));
    }
}
