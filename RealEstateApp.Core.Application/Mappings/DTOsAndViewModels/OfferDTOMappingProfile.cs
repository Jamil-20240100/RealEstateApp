using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Offer;
using RealEstateApp.Core.Application.ViewModels.Client;

namespace RealEstateApp.Core.Application.Mapping
{
    public class DtoToViewModelProfile : Profile
    {
        public DtoToViewModelProfile()
        {
            CreateMap<OfferDTO, OfferViewModel>();

        }
    }
}
