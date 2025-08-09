using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Email;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Helpers;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class PropertyTypeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;

        public PropertyTypeController(
            IPropertyTypeService propertyTypeService, 
            IMapper mapper, UserManager<AppUser> userManager, 
            IAccountServiceForWebApp accountServiceForWebApp, IPropertyService propertyService)
        {
            _propertyTypeService = propertyTypeService;
            _propertyService = propertyService;
            _mapper = mapper;
            _userManager = userManager;
            _accountServiceForWebApp = accountServiceForWebApp;
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
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dtos = await _propertyTypeService.GetAll();
            var types = _mapper.Map<List<PropertyTypeViewModel>>(dtos);

            var allProperties = await _propertyService.GetAll();

            foreach (var type in types)
            {
                type.NumberOfProperties = allProperties.Count(p => p.PropertyType.Id == type.Id);
            }

            return View(types);
        }

        public async Task<IActionResult> Create()
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            ViewBag.EditMode = false;
            ViewBag.ActionName = "Create";

            return View("Save", new SavePropertyTypeViewModel 
            {
                Id = 0, 
                Description = "", 
                Name = ""}
            );
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePropertyTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            
            var dto = _mapper.Map<PropertyTypeDTO>(vm);
            await _propertyTypeService.AddAsync(dto);

            return RedirectToRoute(new { controller = "PropertyType", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return RedirectToRoute(new { controller = "PropertyType", action = "Index" });
            
            var propertyType = await _propertyTypeService.GetById(id);

            if (propertyType == null)
                return RedirectToRoute(new { controller = "PropertyType", action = "Index" });

            var vm = _mapper.Map<SavePropertyTypeViewModel>(propertyType);

            ViewBag.EditMode = true;
            ViewBag.ActionName = "Edit";

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePropertyTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return View(vm);

            var checkDTO = await _propertyTypeService.GetById(vm.Id);

            if (checkDTO == null)
            {
                ViewData["ErrorMessage"] = "Tipo de propiedad no encontrado.";
                return View(vm);
            }

            await _propertyTypeService.UpdateAsync(checkDTO, checkDTO.Id);

            return RedirectToRoute(new { controller = "PropertyType", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return RedirectToRoute(new { controller = "PropertyType", action = "Index" });

            var dto = await _propertyTypeService.GetById(id);

            if (dto == null)
                return RedirectToRoute(new { controller = "PropertyType", action = "Index" });

            DeletePropertyTypeViewModel vm = new()
            {
                Id = dto.Id,
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeletePropertyTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return View(vm);

            await _propertyTypeService.DeleteAsync(vm.Id);

            return RedirectToRoute(new { controller = "PropertyType", action = "Index" });
        }
    }
}
