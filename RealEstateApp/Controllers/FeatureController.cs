using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    public class FeatureController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAccountServiceForWebApp _accountService;
        private readonly IFeatureService _featureService;
        private readonly IMapper _mapper;

        public FeatureController(
            IFeatureService featureService,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IAccountServiceForWebApp accountService)
        {
            _featureService = featureService;
            _mapper = mapper;
            _userManager = userManager;
            _accountService = accountService;
        }

        private async Task<UserViewModel?> ValidateUserAsync()
        {
            var userSession = await _userManager.GetUserAsync(User);
            if (userSession == null) return null;

            var user = await _accountService.GetUserByUserName(userSession.UserName ?? "");
            var mappedUser = _mapper.Map<UserViewModel>(user);
            return mappedUser;
        }

        public async Task<IActionResult> Index()
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var dtos = await _featureService.GetAll();
            var features = _mapper.Map<List<FeatureViewModel>>(dtos);
            return View(features);
        }

        public async Task<IActionResult> Create()
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            ViewBag.EditMode = false;
            ViewBag.ActionName = "Create";

            return View("Save", new SaveFeatureViewModel() { Id = 0, Description = "", Name = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveFeatureViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<FeatureDTO>(vm);
            await _featureService.AddAsync(dto);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var feature = await _featureService.GetById(id);
            if (feature == null)
                return RedirectToAction("Index");

            var vm = _mapper.Map<SaveFeatureViewModel>(feature);
 
            ViewBag.EditMode = true;
            ViewBag.ActionName = "Edit";

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SaveFeatureViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            if (!ModelState.IsValid) return View(vm);

            var existingFeature = await _featureService.GetById(vm.Id);
            if (existingFeature == null)
            {
                ViewData["ErrorMessage"] = "Característica no encontrada.";
                return View(vm);
            }

            await _featureService.UpdateAsync(_mapper.Map<FeatureDTO>(vm), vm.Id);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var feature = await _featureService.GetById(id);
            if (feature == null)
                return RedirectToAction("Index");

            var vm = new DeleteFeatureViewModel { Id = feature.Id };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeleteFeatureViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user == null)
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            await _featureService.DeleteAsync(vm.Id);
            return RedirectToAction("Index");
        }
    }
}
