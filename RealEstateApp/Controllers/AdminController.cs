using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IMapper _mapper;

        public AdminController(IAccountServiceForWebApp accountServiceForWebApp, IMapper mapper, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _accountServiceForWebApp = accountServiceForWebApp;
            _mapper = mapper;
        }

        private async Task<UserViewModel?> ValidateUserAsync()
        {
            var userSession = await _userManager.GetUserAsync(User);
            if (userSession == null) return null;

            var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");
            var mappedUser = _mapper.Map<UserViewModel>(user);
            return mappedUser;
        }

        public async Task<IActionResult> Index()
        {
            var user = await ValidateUserAsync();
            if (user == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            var dtos = await _accountServiceForWebApp.GetAllUserByRole(Roles.Admin.ToString());
            var admins = _mapper.Map<List<UserViewModel>>(dtos);
            ViewBag.LoggedInAdminId = user.Id;

            return View(admins);
        }

        public IActionResult Create()
        {
            var viewModel = new SaveAdminViewModel();
            return View("SaveAdmin", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveAdminViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("SaveAdmin", vm);
            }

            var origin = Request.Headers["origin"];
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Role = Roles.Admin.ToString();
            dto.IsActive = true;

            var result = await _accountServiceForWebApp.RegisterUser(dto, origin);

            if (!result.HasError)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("SaveAdmin", vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var loggedInUser = await ValidateUserAsync();
            if (loggedInUser == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (loggedInUser.Id == id)
            {
                TempData["ErrorMessage"] = "No puedes editar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _accountServiceForWebApp.GetUserById(id);
            if (admin == null) return NotFound();

            var viewModel = _mapper.Map<SaveAdminViewModel>(admin);
            return View("SaveAdmin", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveAdminViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("SaveAdmin", vm);
            }

            var origin = Request.Headers["origin"];
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Role = Roles.Admin.ToString();

            var result = await _accountServiceForWebApp.EditUser(dto, origin);

            if (!result.HasError)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("SaveAdmin", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var loggedInUser = await ValidateUserAsync();
            if (loggedInUser == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (loggedInUser.Id == id)
            {
                TempData["ErrorMessage"] = "No puedes cambiar el estado de tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _accountServiceForWebApp.GetUserById(id);
            if (admin == null) return NotFound();

            bool newStatus = !admin.IsActive;

            await _accountServiceForWebApp.ChangeStatusAsync(id, newStatus);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ConfirmToggleStatus(string id)
        {
            var loggedInUser = await ValidateUserAsync();
            if (loggedInUser == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (loggedInUser.Id == id)
            {
                TempData["ErrorMessage"] = "No puedes cambiar el estado de tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _accountServiceForWebApp.GetUserById(id);
            if (admin == null) return NotFound();

            var viewModel = _mapper.Map<UserViewModel>(admin);
            return View("ConfirmToggleStatus", viewModel);
        }

        public async Task<IActionResult> DeleteConfirmation(string id)
        {
            var loggedInUser = await ValidateUserAsync();
            if (loggedInUser == null)
            {
                return RedirectToRoute(new { controller = "Login", action = "Index" });
            }

            if (loggedInUser.Id == id)
            {
                TempData["ErrorMessage"] = "No puedes eliminar tu propio usuario.";
                return RedirectToAction(nameof(Index));
            }

            var admin = await _accountServiceForWebApp.GetUserById(id);
            if (admin == null) return NotFound();

            var viewModel = _mapper.Map<UserViewModel>(admin);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await _accountServiceForWebApp.GetUserById(id);
            if (admin == null) return NotFound();

            await _accountServiceForWebApp.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}