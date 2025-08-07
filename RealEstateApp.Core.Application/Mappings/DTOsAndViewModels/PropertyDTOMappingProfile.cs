using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.DTOs.Offer;
using RealEstateApp.Core.Application.DTOs.Client;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;


namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class PropertyDTOMappingProfile : Profile
    {
        public PropertyDTOMappingProfile()
        {
            CreateMap<PropertyDTO, PropertyViewModel>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()));

            CreateMap<PropertyViewModel, PropertyDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(url => new PropertyImageDTO { ImageUrl = url }).ToList()));

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
                .ForMember(dest => dest.AgentId, opt => opt.MapFrom(src => src.AgentId));

            CreateMap<PropertyDTO, SavePropertyViewModel>()
                .ForMember(dest => dest.PropertyTypeId, opt => opt.MapFrom(src => src.PropertyType.Id))
                .ForMember(dest => dest.SalesTypeId, opt => opt.MapFrom(src => src.SalesType.Id))
                .ForMember(dest => dest.SelectedFeatures, opt => opt.MapFrom(src => src.Features.Select(f => f.Id).ToList()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(img => img.ImageUrl).ToList()))
                .ForMember(dest => dest.PropertyTypes, opt => opt.Ignore())
                .ForMember(dest => dest.SalesTypes, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableFeatures, opt => opt.Ignore());

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
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<PropertyDTO, PropertyDetailsViewModel>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.ImageUrl).ToList()))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features))
            .ForMember(dest => dest.PropertyType, opt => opt.MapFrom(src => src.PropertyType))
            .ForMember(dest => dest.SalesType, opt => opt.MapFrom(src => src.SalesType))
            // Mapear colecciones de ofertas, mensajes y clientes con ofertas
            .ForMember(dest => dest.Offers, opt => opt.MapFrom(src => src.Offers))  // Asumiendo que PropertyDTO tiene Offers
            .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages)) // Asumiendo que PropertyDTO tiene Messages
            .ForMember(dest => dest.ClientsWithOffers, opt => opt.MapFrom(src => src.ClientsWithOffers)) // Similar con clientes que hicieron ofertas
            .ForMember(dest => dest.ClientsWhoMessaged, opt => opt.Ignore()) // Si tienes esa propiedad, mapéala también aquí o ignórala si no existe
            ;

            // Aquí asumes que existen mapeos para las clases anidadas, por ejemplo:
            CreateMap<PropertyTypeDTO, PropertyTypeViewModel>();
            CreateMap<SalesTypeDTO, SalesTypeViewModel>();
            CreateMap<FeatureDTO, FeatureViewModel>();

            CreateMap<OfferDTO, OfferViewModel>();
            CreateMap<MessageDTO, MessageViewModel>();

            CreateMap<ClientDTO, ClientsWhoMadeOfferViewModel>(); // O el DTO que uses para clientes que hicieron oferta

        }
    }
}
