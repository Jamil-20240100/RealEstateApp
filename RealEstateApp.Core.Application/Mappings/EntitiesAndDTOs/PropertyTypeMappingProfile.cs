using AutoMapper;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Create;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Update;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class PropertyTypeMappingProfile : Profile
    {
        public PropertyTypeMappingProfile()
        {
            // Entity <-> DTO principal
            CreateMap<PropertyType, PropertyTypeDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.MapFrom(src => src.Properties != null ? src.Properties.Count : 0))
                .ReverseMap()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());

            // Para API CQRS (Create Command -> Entity)
            CreateMap<CreatePropertyTypeCommand, PropertyType>();

            // Para API CQRS (Update Command -> Entity)
            CreateMap<UpdatePropertyTypeCommand, PropertyType>();

            // Entity <-> DTOs de creación/actualización directa (por si se usan en API sin CQRS)
            CreateMap<PropertyType, CreatePropertyTypeDTO>().ReverseMap();
            CreateMap<PropertyType, UpdatePropertyTypeDTO>().ReverseMap();
        }
    }
}
