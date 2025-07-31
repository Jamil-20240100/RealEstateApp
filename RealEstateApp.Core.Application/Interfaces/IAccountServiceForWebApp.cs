using RealEstateApp.Core.Application.DTOs.User;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApp : IBaseAccountService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
        Task SignOutAsync();
        Task<UserDto?> GetUserById(string userId);
        Task<string?> GetUserEmailAsync(string userId);
        Task<string> GetUserFullNameAsync(string userId);
    }
}