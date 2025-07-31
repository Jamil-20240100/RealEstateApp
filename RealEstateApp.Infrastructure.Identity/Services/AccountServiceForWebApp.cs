using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp : BaseAccountService, IAccountServiceForWebApp
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AccountServiceForWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IMapper mapper) : base(userManager, emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailService = emailService;
        }

        // Authenticate a user

        public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
        {
            var response = new LoginResponseDto
            {
                Email = "",
                Id = "",
                LastName = "",
                Name = "",
                UserName = "",
                HasError = false,
                Errors = new List<string>()
            };

            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"There is no account registered with this username: {loginDto.UserName}");
                return response;
            }

            if (!user.EmailConfirmed)
            {
                response.HasError = true;
                response.Errors.Add($"This account {loginDto.UserName} is not active, you should check your email.");
                return response;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
            {
                response.HasError = true;
                if (result.IsLockedOut)
                {
                    response.Errors.Add($"Your account {loginDto.UserName} has been locked due to multiple failed attempts." +
                                         " Please try again in 10 minutes. If you don’t remember your password, you can go through the password reset process.");
                }
                else
                {
                    response.Errors.Add($"These credentials are invalid for this user: {user.UserName}");
                }
                return response;
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            response.Id = user.Id;
            response.Email = user.Email ?? "";
            response.UserName = user.UserName ?? "";
            response.Name = user.Name;
            response.LastName = user.LastName;
            response.IsVerified = user.EmailConfirmed;
            response.Roles = rolesList.ToList();

            return response;
        }

        // Sign out a user
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // Get user by ID
        public async Task<UserDto?> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? "Unknown",
                Name = user.Name ?? "Unknown",
                LastName = user.LastName ?? "Unknown",
                Email = user.Email ?? "Unknown",
                Role = roles.FirstOrDefault() ?? "Unknown",
                IsActive = user.IsActive,
                isVerified = user.EmailConfirmed,
                ProfileImage = user.ProfileImage,
                PhoneNumber = user.PhoneNumber
            };
        }

        // Get user by Username
        public async Task<AppUser?> GetUserByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        // Get user's email by user ID
        public async Task<string?> GetUserEmailAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }

        // Get user's full name by user ID
        public async Task<string> GetUserFullNameAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user == null ? "" : $"{user.Name} {user.LastName}";
        }       
    }
}
