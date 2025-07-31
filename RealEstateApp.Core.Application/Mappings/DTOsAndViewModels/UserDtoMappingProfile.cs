using AutoMapper;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.ViewModels.User;

namespace RealEstateApp.Core.Application.Mappings.DTOsAndViewModels
{
    public class UserDtoMappingProfile : Profile
    {
        public UserDtoMappingProfile()
        {
            CreateMap<UserDto, UserViewModel>()
                .ReverseMap();

            CreateMap<UserDto, DeleteUserViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ReverseMap()
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore());


            CreateMap<UserDto, UpdateUserViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<SaveUserDto, UpdateUserViewModel>()
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<SaveUserDto, CreateUserViewModel>()
                .ReverseMap();

            CreateMap<SaveUserDto, RegisterUserViewModel>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ReverseMap();

            CreateMap<LoginDto, LoginViewModel>()
                .ReverseMap();
        }
    }
}