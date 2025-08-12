using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.DTOs.Email;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateApp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApi : BaseAccountService, IAccountServiceForWebApi
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        public AccountServiceForWebApi(
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager, 
            IEmailService emailService, 
            IOptions<JwtSettings> jwtSettings)
            : base(userManager, emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        public async Task<LoginResponseForApiDTO?> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null || !user.EmailConfirmed)
                return null;

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
                return null;

            JwtSecurityToken jwtSecurityToken = await GenerateJwtToken(user);

            return new LoginResponseForApiDTO
            {
                Jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken)
            };
        }

        public async Task<bool> ConfirmAccountAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null || string.IsNullOrWhiteSpace(token))
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public override async Task<RegisterResponseDto> RegisterUser(SaveUserDto saveDto, string? origin, bool? isApi = false)
        {
            return await base.RegisterUser(saveDto, null, isApi);
        }

        public override async Task<EditResponseDto> EditUser(SaveUserDto saveDto, string? origin, bool? isCreated = false, bool? isApi = false)
        {
            return await base.EditUser(saveDto, null, isCreated, isApi);
        }

        public override async Task<UserResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false)
        {
            return await base.ForgotPasswordAsync(request, isApi);
        }

        public async Task<UserResponseDto> ForgotPasswordWithTokenAsync(ForgotPasswordWithTokenDto request)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add($"There is no account registered with this username {request.UserName}");
                return response;
            }

            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            var resetToken = await GetResetPasswordToken(user);
            await _emailService.SendAsync(new EmailRequestDto()
            {
                To = user.Email,
                HtmlBody = $"Please reset your password using this token: {resetToken}",
                Subject = "Reset password"
            });

            return response;
        }

        public async Task<UserResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            UserResponseDto response = new() { HasError = false, Errors = [] };

            if (string.IsNullOrWhiteSpace(request.UserId) ||
                string.IsNullOrWhiteSpace(request.Token) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                response.HasError = true;
                response.Errors.Add("All fields are required.");
                return response;
            }

            if (request.Password != request.ConfirmPassword)
            {
                response.HasError = true;
                response.Errors.Add("Password and Confirm Password do not match.");
                return response;
            }

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                response.HasError = true;
                response.Errors.Add("User not found.");
                return response;
            }

            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);
            if (!resetResult.Succeeded)
            {
                response.HasError = true;
                response.Errors.AddRange(resetResult.Errors.Select(e => e.Description));
                return response;
            }

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return response;
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateUserAsync(CreateUserRequestDto request, Roles rol)
        {
            if (await _userManager.FindByNameAsync(request.Usuario) != null)
                return (false, "El usuario ya está registrado.");

            if (await _userManager.FindByEmailAsync(request.Correo) != null)
                return (false, "El correo ya está registrado.");

            var user = new AppUser
            {
                Name = request.Nombre,
                LastName = request.Apellido,
                UserName = request.Usuario,
                Email = request.Correo,
                EmailConfirmed = true,
                IsActive = true,
                ProfileImage = "",
                PhoneNumber = "",
                UserIdentification = request.Cedula,
            };

            var createResult = await _userManager.CreateAsync(user, request.Contrasena);

            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, rol.ToString().ToLower());

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return (true, null);
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Unknown";

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Role = role,
                IsActive = user.IsActive,
                isVerified = null,
                ProfileImage = user.ProfileImage,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task UpdateUserAsync(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.Id);
            if (user != null)
            {
                user.IsActive = userDto.IsActive;
                await _userManager.UpdateAsync(user);
            }
        }

        #region "private methods"
        private async Task<JwtSecurityToken> GenerateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var rolesClaims = new List<Claim>();
            foreach (var role in roles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("uid",user.Id ?? "")
            }.Union(userClaims).Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }

        #endregion
    }
}