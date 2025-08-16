using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Client;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.DTOs.Offer;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class PropertyMappingProfile : Profile
    {
        public PropertyMappingProfile()
        {
            CreateMap<Property, PropertyDTO>()
                .ForMember(dest => dest.IsFavorite, opt => opt.Ignore()) // O mapea según tu lógica
                .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => src.State == PropertyState.Vendida))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType))
                .ForMember(dest => dest.SalesType, opt => opt.MapFrom(src => src.SalesType))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.Offers, opt => opt.MapFrom(src => src.Offers))
                .ForMember(dest => dest.ClientsWithOffers, opt => opt.Ignore()); // O mapea según tu lógica


            CreateMap<PropertyDTO, Property>()
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.BuyerClientId, opt => opt.MapFrom(src => src.BuyerClientId));

            CreateMap<PropertyImage, PropertyImageDTO>().ReverseMap();

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
                    Description = src.SalesType.Description
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


            CreateMap<PropertyType, PropertyTypeDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore()); // Si no existe en el DTO

            CreateMap<SalesType, SalesTypeDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore()); // Si no existe en el DTO
            CreateMap<Feature, FeatureDTO>();
            CreateMap<Offer, OfferDTO>();
            CreateMap<Message, MessageDTO>();
        }
    }
}
