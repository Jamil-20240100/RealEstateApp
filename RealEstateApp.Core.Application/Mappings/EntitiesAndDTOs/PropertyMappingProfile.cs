using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Property, PropertyDTO>()
    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            CreateMap<PropertyDTO, Property>()
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            CreateMap<PropertyImage, PropertyImageDTO>().ReverseMap();

            CreateMap<Property, PropertyDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State)); // Asumiendo que PropertyDTO tiene State


            CreateMap<Property, PropertyDTO>()
    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
    .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
    .ForMember(dest => dest.BuyerClientId, opt => opt.MapFrom(src => src.BuyerClientId));  // <-- nuevo

            CreateMap<PropertyDTO, Property>()
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.BuyerClientId, opt => opt.MapFrom(src => src.BuyerClientId));

            //
            // mapper for API
            //

            CreateMap<Property, PropertyForApiDTO>()
                
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => new PropertyTypeDTO
                {
                    Id = src.PropertyTypeId,
                    Name = src.PropertyType.Name, 
                    Description = src.PropertyType.Description
                }))

                .ForMember(dest => dest.SalesType, opt => opt.MapFrom(src => new SalesTypeDTO 
                {
                    Id = src.SalesTypeId,
                    Name = src.SalesType.Name,
                    Description= src.SalesType.Description
                }))

                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features.Select(f => new FeatureDTO 
                {
                    Id = f.Id,
                    Name = f.Name,
                    Description = f.Description
                }).ToList()));

            CreateMap<PropertyForApiDTO, Property>()
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.BuyerClientId, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.Ignore());
        }
    }
}
