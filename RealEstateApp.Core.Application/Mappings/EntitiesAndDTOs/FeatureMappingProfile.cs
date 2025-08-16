using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Features.Features.Commands.Create;
using RealEstateApp.Core.Application.Features.Features.Commands.Update;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs
{
    public class FeatureMappingProfile : Profile
    {
        public FeatureMappingProfile()
        {
            // Entity ↔ DTO
            CreateMap<Feature, FeatureDTO>()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.MapFrom(src => src.Properties != null ? src.Properties.Count : 0))
                .ReverseMap()
                .ForMember(dest => dest.Properties, opt => opt.Ignore());

            // Commands → Entity
            CreateMap<CreateFeatureCommand, Feature>();
            CreateMap<UpdateFeatureCommand, Feature>();

            // Entity ↔ DTOs directos
            CreateMap<Feature, CreateFeatureDTO>().ReverseMap();
            CreateMap<Feature, UpdateFeatureDTO>().ReverseMap();
        }
    }
}
