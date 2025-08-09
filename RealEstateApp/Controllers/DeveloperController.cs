using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.User;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class DeveloperController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IMapper _mapper;

        public DeveloperController(IAccountServiceForWebApp accountServiceForWebApp, IMapper mapper, UserManager<AppUser> userManager)
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
            if (user == null) return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dtos = await _accountServiceForWebApp.GetAllUserByRole(Roles.Developer.ToString());
            var devs = _mapper.Map<List<UserViewModel>>(dtos);
            return View(devs);
        }

        public IActionResult Create()
        {
            var viewModel = new SaveUserViewModel(); // Usa un VM con Nombre, Apellido, Cedula, Usuario, Correo, Contraseña
            return View("SaveDeveloper", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("SaveDeveloper", vm);

            var origin = Request.Headers["origin"];
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Role = Roles.Developer.ToString();
            dto.IsActive = true;

            var result = await _accountServiceForWebApp.RegisterUser(dto, origin);

            if (!result.HasError)
                return RedirectToAction(nameof(Index));

            // Agrega todos los errores que venga en la lista
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return View("SaveDeveloper", vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var loggedUser = await ValidateUserAsync();
            if (loggedUser == null) return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dev = await _accountServiceForWebApp.GetUserById(id);
            if (dev == null) return NotFound();

            var vm = _mapper.Map<SaveUserViewModel>(dev);
            return View("SaveDeveloper", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveUserViewModel vm)
        {
            if (!ModelState.IsValid) return View("SaveDeveloper", vm);

            var origin = Request.Headers["origin"];
            var dto = _mapper.Map<SaveUserDto>(vm);
            dto.Role = Roles.Developer.ToString();

            var result = await _accountServiceForWebApp.EditUser(dto, origin);
            if (!result.HasError) return RedirectToAction(nameof(Index));

            return View("SaveDeveloper", vm);
        }

        public async Task<IActionResult> ConfirmToggleStatus(string id)
        {
            var dev = await _accountServiceForWebApp.GetUserById(id);
            if (dev == null) return NotFound();
            var vm = _mapper.Map<UserViewModel>(dev);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var dev = await _accountServiceForWebApp.GetUserById(id);
            if (dev == null) return NotFound();

            bool newStatus = !dev.IsActive;
            await _accountServiceForWebApp.ChangeStatusAsync(id, newStatus);

            return RedirectToAction(nameof(Index));
        }
    }
}
