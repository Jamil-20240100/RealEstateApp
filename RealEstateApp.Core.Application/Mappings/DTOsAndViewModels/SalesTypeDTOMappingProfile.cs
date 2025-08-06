using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.SalesType;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class SalesTypeDTOMappingProfile : Profile
    {
        public SalesTypeDTOMappingProfile()
        {
            CreateMap<SalesTypeDTO, SalesTypeViewModel>().ReverseMap();

            CreateMap<SalesTypeDTO, SaveSalesTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());

            CreateMap<SalesTypeDTO, DeleteSalesTypeViewModel>().ReverseMap()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.NumberOfProperties, opt => opt.Ignore());
        }
    }
}
