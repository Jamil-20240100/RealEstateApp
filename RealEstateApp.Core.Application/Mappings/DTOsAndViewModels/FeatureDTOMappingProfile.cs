using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.ViewModels.Feature;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class FeatureDTOMappingProfile : Profile
    {
        public FeatureDTOMappingProfile()
        {
            CreateMap<FeatureDTO, FeatureViewModel>().ReverseMap();

            CreateMap<FeatureDTO, SaveFeatureViewModel>().ReverseMap()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());

            CreateMap<FeatureDTO, DeleteFeatureViewModel>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());
        }
    }
}
