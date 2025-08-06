using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class SalesTypeMappingProfile : Profile
    {
        public SalesTypeMappingProfile()
        {
            CreateMap<SalesType, SalesTypeDTO>()
               .ForMember(dest => dest.NumberOfProperties, opt => opt.MapFrom(src => src.Properties != null ? src.Properties.Count : 0))
               .ReverseMap()
               .ForMember(dest => dest.Properties, opt => opt.Ignore());
        }
    }
}
