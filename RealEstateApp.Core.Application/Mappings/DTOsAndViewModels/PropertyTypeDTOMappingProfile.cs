using AutoMapper;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.ViewModels.PropertyType;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class PropertyTypeDTOMappingProfile : Profile
    {
        public PropertyTypeDTOMappingProfile()
        {
            CreateMap<PropertyTypeDTO, PropertyTypeViewModel>().ReverseMap();
            
            CreateMap<PropertyTypeDTO, SavePropertyTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());

            CreateMap<PropertyTypeDTO, DeletePropertyTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());
        }
    }
}
