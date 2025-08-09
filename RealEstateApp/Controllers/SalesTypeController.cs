using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Helpers;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class SalesTypeController : Controller
    {
        private readonly ISalesTypeService _salesTypeService;
        IPropertyService _propertyService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;

        public SalesTypeController(
            ISalesTypeService salesTypeService,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IAccountServiceForWebApp accountServiceForWebApp, IPropertyService propertyService)
        {
            _propertyService = propertyService;
            _salesTypeService = salesTypeService;
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

            var dtos = await _salesTypeService.GetAll();
            var types = _mapper.Map<List<SalesTypeViewModel>>(dtos);
            var allProperties = await _propertyService.GetAll();

            foreach (var type in types)
            {
                type.NumberOfProperties = allProperties.Count(p => p.SalesType.Id == type.Id);
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

            return View("Save", new SaveSalesTypeViewModel() 
            {
                Id = 0,
                Description = "",
                Name = ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveSalesTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return View(vm);

            await _salesTypeService.AddAsync(_mapper.Map<SalesTypeDTO>(vm));
            return RedirectToRoute(new { controller = "SalesType", action = "Index" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dto = await _salesTypeService.GetById(id);
            if (dto == null)
                return RedirectToRoute(new { controller = "SalesType", action = "Index" });

            ViewBag.EditMode = true;
            ViewBag.ActionName = "Edit";

            return View("Save", _mapper.Map<SaveSalesTypeViewModel>(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveSalesTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return View(vm);

            var dto = await _salesTypeService.GetById(vm.Id);
            if (dto == null)
            {
                ViewData["ErrorMessage"] = "Tipo de venta no encontrado.";
                return View(vm);
            }

            await _salesTypeService.UpdateAsync(dto, dto.Id);
            return RedirectToRoute(new { controller = "SalesType", action = "Index" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dto = await _salesTypeService.GetById(id);
            if (dto == null)
                return RedirectToRoute(new { controller = "SalesType", action = "Index" });

            return View(new DeleteSalesTypeViewModel { Id = dto.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteSalesTypeViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid)
                return View(vm);

            await _salesTypeService.DeleteAsync(vm.Id);
            return RedirectToRoute(new { controller = "SalesType", action = "Index" });
        }
    }
}