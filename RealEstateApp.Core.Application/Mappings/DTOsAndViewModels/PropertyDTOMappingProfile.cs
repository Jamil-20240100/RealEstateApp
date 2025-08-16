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
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class PropertyDTOMappingProfile : Profile
    {
        public PropertyDTOMappingProfile()
        {
            // PropertyDTO -> PropertyViewModel
            CreateMap<PropertyDTO, PropertyViewModel>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => src.State == PropertyState.Vendida))
                .ForMember(dest => dest.BuyerClientId, opt => opt.MapFrom(src => src.BuyerClientId))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType))
                .ForMember(dest => dest.SalesType, opt => opt.MapFrom(src => src.SalesType))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features))
                .ForMember(dest => dest.IsBoughtByCurrentClient, opt => opt.Ignore()); // Si tienes lógica especial, cámbiala

            // PropertyViewModel -> PropertyDTO
            CreateMap<PropertyViewModel, PropertyDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(url => new PropertyImageDTO { ImageUrl = url }).ToList()))
                .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType))
                .ForMember(dest => dest.SalesType, opt => opt.MapFrom(src => src.SalesType))
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features))
                .ForMember(dest => dest.IsSold, opt => opt.MapFrom(src => src.IsSold))
                .ForMember(dest => dest.Offers, opt => opt.Ignore()) // Normalmente no se mapea desde VM
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.ClientsWithOffers, opt => opt.Ignore());

            // SavePropertyViewModel -> PropertyDTO
            CreateMap<SavePropertyViewModel, PropertyDTO>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.SelectedFeatures.Select(id => new FeatureDTO { Id = id }).ToList()))
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(url => new PropertyImageDTO { ImageUrl = url }).ToList()))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.SizeInMeters, opt => opt.MapFrom(src => src.SizeInMeters))
                .ForMember(dest => dest.NumberOfRooms, opt => opt.MapFrom(src => src.NumberOfRooms))
                .ForMember(dest => dest.NumberOfBathrooms, opt => opt.MapFrom(src => src.NumberOfBathrooms))
                .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId))
                .ForMember(dest => dest.IsFavorite, opt => opt.Ignore())
                .ForMember(dest => dest.IsSold, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.ClientsWithOffers, opt => opt.Ignore());

            // PropertyDTO -> SavePropertyViewModel
            CreateMap<PropertyDTO, SavePropertyViewModel>()
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.SelectedFeatures, opt => opt.MapFrom(src => src.Features.Select(f => f.Id).ToList()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.PropertyTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SalesTypes, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableFeatures, opt => opt.Ignore());

            // PropertyDTO <-> DeletePropertyViewModel
            CreateMap<PropertyDTO, DeletePropertyViewModel>().ReverseMap()
                .ForMember(dest => dest.PropertyType, opt => opt.Ignore())
                .ForMember(dest => dest.SalesType, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.SizeInMeters, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfRooms, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfBathrooms, opt => opt.Ignore())
                .ForMember(dest => dest.Features, opt => opt.Ignore())
                .ForMember(dest => dest.Code, opt => opt.Ignore())
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.IsFavorite, opt => opt.Ignore())
                .ForMember(dest => dest.IsSold, opt => opt.Ignore())
                .ForMember(dest => dest.Offers, opt => opt.Ignore())
                .ForMember(dest => dest.Messages, opt => opt.Ignore())
                .ForMember(dest => dest.ClientsWithOffers, opt => opt.Ignore());

            // PropertyDTO -> PropertyDetailsViewModel
            CreateMap<PropertyDTO, PropertyDetailsViewModel>()
                .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features ?? new List<FeatureDTO>()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl) ?? new List<string>()))
                .ForMember(dest => dest.Offers, opt => opt.MapFrom(src => src.Offers ?? new List<OfferDTO>()))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages ?? new List<MessageDTO>()))
                .ForMember(dest => dest.ClientsWithOffers, opt => opt.MapFrom(src => src.ClientsWithOffers ?? new List<ClientDTO>()))
                .ForMember(dest => dest.ClientsWhoMessaged, opt => opt.Ignore())
                .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId))
                .ForMember(dest => dest.AgentName, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedClientId, opt => opt.Ignore());

            // Mappings for nested types
            CreateMap<PropertyTypeDTO, PropertyTypeViewModel>();
            CreateMap<SalesTypeDTO, SalesTypeViewModel>();
            CreateMap<FeatureDTO, FeatureViewModel>();
            CreateMap<OfferDTO, OfferViewModel>();
            CreateMap<MessageDTO, MessageViewModel>();
            CreateMap<ClientDTO, ClientsWhoMadeOfferViewModel>();
        }
    }
}