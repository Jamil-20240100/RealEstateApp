using RealEstateApp.Core.Application.DTOs.User;
namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<LoginResponseForApiDTO> AuthenticateAsync(LoginDto loginDto);
        Task<bool> ConfirmAccountAsync(string userId, string token);
        Task<UserResponseDto> ForgotPasswordWithTokenAsync(ForgotPasswordWithTokenDto request);
        Task<(bool Success, string? ErrorMessage)> CreateUserAsync(CreateUserRequestDto request);
        Task UpdateUserAsync(UserDto userDto);
        Task<UserDto> GetUserByIdAsync(string userId);
    }
}