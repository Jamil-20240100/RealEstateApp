using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [SwaggerTag("Endpoints for user authentication and user registration (Admin and Developer)")]
    public class AccountController : BaseApiController
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;
        public AccountController(IAccountServiceForWebApi accountServiceForWebApi)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(
            Summary = "Authenticate user",
            Description = "Validates user credentials and returns an authentication token with user information"
        )]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [SwaggerOperation(
            Summary = "Register a new administrator",
            Description = "Creates a new user account with the role Admin"
        )]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [SwaggerOperation(
            Summary = "Register a new developer",
            Description = "Creates a new user account with the role Developer"
        )]
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
