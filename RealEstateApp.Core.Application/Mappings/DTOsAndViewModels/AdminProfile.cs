using AutoMapper;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Core.Application.ViewModels.User;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class AdminProfile : Profile
    {
        public AdminProfile()
        {
            CreateMap<UserDto, UserViewModel>()
                .ForMember(dest => dest.PropertiesQuantity, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UserDto, SaveAdminViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<SaveAdminViewModel, SaveUserDto>()
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}