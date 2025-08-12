using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.User;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class AgentForApiDTOMappingProfile : Profile
    {
        public AgentForApiDTOMappingProfile()
        {
            CreateMap<AgentForApiDTO, UserDto>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.isVerified, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.UserIdentification, opt => opt.Ignore());

            CreateMap<UserDto, AgentForApiDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());
        }
    }
}
