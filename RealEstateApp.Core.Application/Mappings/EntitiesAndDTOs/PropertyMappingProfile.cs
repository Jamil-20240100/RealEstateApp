using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<Property, PropertyViewModel>()
                .ForMember(dest => dest.IsFavorite, opt => opt.Ignore())
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.PropertyType.Name))   // ✅ Mapeo del nombre del PropertyType
                .ForMember(dest => dest.SaleType,
                    opt => opt.MapFrom(src => src.SaleType.Name));     // ✅ Mapeo del nombre del SaleType

            // Mapeo inverso si lo necesitas
            CreateMap<PropertyViewModel, Property>();
        }
    }
}
