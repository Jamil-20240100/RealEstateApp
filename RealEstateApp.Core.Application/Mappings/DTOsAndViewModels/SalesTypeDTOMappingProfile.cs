using AutoMapper;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.ViewModels.SalesType;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class SalesTypeDTOMappingProfile : Profile
    {
        public SalesTypeDTOMappingProfile()
        {
            // Mapping entre SalesTypeDTO y SalesTypeViewModel
            CreateMap<SalesTypeDTO, SalesTypeViewModel>().ReverseMap();

            // Mapping entre SalesTypeDTO y SaveSalesTypeViewModel
            CreateMap<SalesTypeDTO, SaveSalesTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            // Mapping entre SalesTypeDTO y DeleteSalesTypeViewModel
            CreateMap<SalesTypeDTO, DeleteSalesTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());
        }
    }
}
