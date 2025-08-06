using AutoMapper;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class PropertyTypeMappingProfile : Profile
    {
        public PropertyTypeMappingProfile()
        {
            CreateMap<PropertyType, PropertyTypeDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.MapFrom(src => src.Properties != null ? src.Properties.Count : 0))
                .ReverseMap()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());
        }
    }
}
