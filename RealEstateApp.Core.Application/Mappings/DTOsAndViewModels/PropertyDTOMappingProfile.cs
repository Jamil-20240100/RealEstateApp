using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.ViewModels.Property;

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
                .ForMember(dest => dest.Images, opt => opt.Ignore());
        }
    }
}
