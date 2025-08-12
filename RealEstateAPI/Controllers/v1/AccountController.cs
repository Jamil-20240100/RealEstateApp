using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;
        public AccountController(IAccountServiceForWebApi accountServiceForWebApi)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrWhiteSpace(loginDto.UserName) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest("Username and password are required.");

            var response = await _accountServiceForWebApi.AuthenticateAsync(loginDto);

            if (response == null)
                return Unauthorized("Invalid credentials or account not confirmed.");

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] CreateUserRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage) = await _accountServiceForWebApi.CreateUserAsync(request, Roles.Admin);

            if (!success)
            {
                if (errorMessage.Contains("registrado"))
                    return Conflict(new { message = errorMessage });

                return BadRequest(new { message = errorMessage });
            }

            return Created("", new { message = "Administrador registrado exitosamente." });
        }

        [Authorize(Roles = "Admin, Developer")]
        [HttpPost("register-developer")]
        public async Task<IActionResult> RegisterDeveloper([FromBody] CreateUserRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, errorMessage) = await _accountServiceForWebApi.CreateUserAsync(request, Roles.Developer);

            if (!success)
            {
                if (errorMessage.Contains("registrado"))
                    return Conflict(new { message = errorMessage });

                return BadRequest(new { message = errorMessage });
            }

            return Created("", new { message = "Desarrollador registrado exitosamente." });
        }
    }
}
