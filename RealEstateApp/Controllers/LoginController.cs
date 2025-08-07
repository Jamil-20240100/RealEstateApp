using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Helpers;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginController(
            IAccountServiceForWebApp accountServiceForWebApp, 
            IMapper mapper, 
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }
       
        public async Task<IActionResult> Index()
        {
            AppUser? userSession = await _userManager.GetUserAsync(User);

            if (userSession != null)
            {
                var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");

                if (user != null && user.Role == Roles.Admin.ToString())
                {
                    return RedirectToRoute(new { controller = "AdminHome", action = "Index" });
                }
                else if (user != null && user.Role == Roles.Client.ToString())
                {
                    return RedirectToRoute(new { controller = "ClientHome", action = "Index" });
                }
                else if (user != null && user.Role == Roles.Agent.ToString())
                {
                    return RedirectToRoute(new { controller = "AgentHome", action = "Index" });
                }
            }

            return View(new LoginViewModel() { Password = "", UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel vm)
        {
            AppUser? userSession = await _userManager.GetUserAsync(User);

            if (userSession != null)
            {
                var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");

                if (user != null && user.Role == Roles.Admin.ToString())
                {
                    return RedirectToRoute(new { controller = "AdminHome", action = "Index" });
                }
                else if (user != null && user.Role == Roles.Client.ToString())
                {
                    return RedirectToRoute(new { controller = "ClientHome", action = "Index" });
                }
                else if (user != null && user.Role == Roles.Agent.ToString())
                {
                    return RedirectToRoute(new { controller = "AgentHome", action = "Index" });
                }
            }

            if (!ModelState.IsValid)
            {
                vm.Password = "";
                return View(vm);
            }

            LoginResponseDto? userDto = await _accountServiceForWebApp.AuthenticateAsync(new LoginDto()
            {
                Password = vm.Password,
                UserName = vm.UserName
            });

            if (userDto != null && !userDto.HasError)
            {
                if (userDto.Roles != null && userDto.Roles.Any(r => r == Roles.Admin.ToString()))
                {
                    return RedirectToRoute(new { controller = "AdminHome", action = "Index" });
                }
                else if (userDto.Roles != null && userDto.Roles.Any(r => r == Roles.Agent.ToString()))
                {
                    return RedirectToRoute(new { controller = "AgentHome", action = "Index" });
                }
                else if (userDto.Roles != null && userDto.Roles.Any(r => r == Roles.Client.ToString()))
                {
                    return RedirectToRoute(new { controller = "ClientHome", action = "Index" });
                }
            }
            else
            {
                foreach (var error in userDto?.Errors ?? [])
                {
                    ModelState.AddModelError("userValidation", error);
                }
            }

            vm.Password = "";
            return View(vm);
        }

        public async Task<IActionResult> Logout()
        {
            await _accountServiceForWebApp.SignOutAsync();
            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public async Task<IActionResult> Register()
        {
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();

            return View(new RegisterUserViewModel()
            {
                ConfirmPassword = "",
                Email = "",
                LastName = "",
                Name = "",
                Password = "",
                UserName = "",
                IsActive = true,
                PhoneNumber = "",
                Role = Roles.Client,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            SaveUserDto dto = _mapper.Map<SaveUserDto>(vm);
            string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;

            RegisterResponseDto? returnUser = await _accountServiceForWebApp.RegisterUser(dto, origin);

            if (returnUser.HasError)
            {
                ViewBag.HasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            if (returnUser != null && !string.IsNullOrWhiteSpace(returnUser.Id))
            {
                dto.Id = returnUser.Id;
                dto.ProfileImage = FileManager.Upload(vm.ProfileImageFile, dto.Id, "Users");
                await _accountServiceForWebApp.EditUser(dto, origin, true);
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            UserResponseDto response = await _accountServiceForWebApp.ConfirmAccountAsync(userId, token);
            return View("ConfirmEmail", response.Message);
        }

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordRequestViewModel() { UserName = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;

            ForgotPasswordRequestDto dto = new() { UserName = vm.UserName, Origin = origin };

            UserResponseDto? returnUser = await _accountServiceForWebApp.ForgotPasswordAsync(dto);

            if (returnUser.HasError)
            {
                ViewBag.HasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordRequestViewModel() { Id = userId, Token = token, Password = "" });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            ResetPasswordRequestDto dto = new() { UserId = vm.Id, Password = vm.Password, Token = vm.Token };

            UserResponseDto? returnUser = await _accountServiceForWebApp.ResetPasswordAsync(dto);

            if (returnUser.HasError)
            {
                ViewBag.HasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public async Task<IActionResult> AccessDenied()
        {
            AppUser? userSession = await _userManager.GetUserAsync(User);

            if (userSession != null)
            {
                var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");
                ViewBag.CurrentRol = user?.Role ?? "";
                return View();
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }
    }
}
