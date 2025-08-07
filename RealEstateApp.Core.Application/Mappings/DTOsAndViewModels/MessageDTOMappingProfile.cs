using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.ViewModels.Client;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class MessageDTOMappingProfile : Profile
    {
        public MessageDTOMappingProfile()
        {
            CreateMap<MessageDTO, MessageViewModel>().ReverseMap();
        }
    }
}
