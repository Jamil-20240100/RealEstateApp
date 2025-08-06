using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.ViewModels.Property;
using AutoMapper;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Feature;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using RealEstateApp.Helpers;

namespace RealEstateApp.Controllers
{
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyTypeService _propertyTypeService;
        private readonly ISalesTypeService _saleTypeService;
        private readonly IFeatureService _featureService;
        private readonly IAccountServiceForWebApp _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public PropertyController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            ISalesTypeService saleTypeService,
            IFeatureService featureService,
            IAccountServiceForWebApp userService,
            IMapper mapper, UserManager<AppUser> userManager
        )
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _saleTypeService = saleTypeService;
            _featureService = featureService;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
        }

        private async Task<UserViewModel?> ValidateUserAsync()
        {
            var userSession = await _userManager.GetUserAsync(User);
            if (userSession == null) return null;

            var user = await _userService.GetUserByUserName(userSession.UserName ?? "");
            var mappedUser = _mapper.Map<UserViewModel>(user);
            return mappedUser;
        }

        public async Task<IActionResult> Index()
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var properties = await _propertyService.GetAllWithInclude();
            var mappedProperties = _mapper.Map<List<PropertyViewModel>>(properties);
            return View(mappedProperties);
        }

        public async Task<IActionResult> Create()
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var vm = new SavePropertyViewModel() { Description = "" };

            await LoadDropdowns(vm);

            if (!vm.PropertyTypes.Any() || !vm.SalesTypes.Any() || !vm.AvailableFeatures.Any())
            {
                TempData["Error"] = "Debe crear al menos un tipo de propiedad, tipo de venta y mejora antes de agregar una propiedad.";
                return RedirectToAction("Index");
            }

            ViewBag.EditMode = false;
            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SavePropertyViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            vm.AgentId = user.Id;

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                ViewBag.EditMode = false;
                return View("Save", vm);
            }

            List<string> uploadedImages = new();
            if (vm.ImagesFile != null && vm.ImagesFile.Any())
            {
                foreach (var file in vm.ImagesFile)
                {
                    var imagePath = FileManager.Upload(file, Guid.NewGuid().ToString(), "Properties");
                    uploadedImages.Add(imagePath);
                }
            }
            else if (vm.Images != null && vm.Images.Any())
            {
                uploadedImages = vm.Images;
            }

            var dto = _mapper.Map<PropertyDTO>(vm);

            dto.Images = uploadedImages
                .Select(url => new PropertyImageDTO { ImageUrl = url })
                .ToList();

            dto.PropertyType = await _propertyTypeService.GetById(vm.PropertyTypeId);
            dto.SalesType = await _saleTypeService.GetById(vm.SalesTypeId);
            dto.Code = await _propertyService.GenerateUniquePropertyCodeAsync();

            await _propertyService.AddAsync(dto);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var property = await _propertyService.GetByIdWithInclude(id);
            if (property == null || property.AgentId != user.Id)
                return RedirectToAction("Index");

            var vm = _mapper.Map<SavePropertyViewModel>(property);
            await LoadDropdowns(vm);
            ViewBag.EditMode = true;

            return View("Save", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SavePropertyViewModel vm)
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            vm.AgentId = user.Id;

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                ViewBag.EditMode = true;
                return View("Save", vm);
            }

            List<string> allImages = vm.Images != null ? new List<string>(vm.Images) : new List<string>();

            if (vm.ImagesFile != null && vm.ImagesFile.Any())
            {
                foreach (var file in vm.ImagesFile)
                {
                    var imagePath = FileManager.Upload(file, Guid.NewGuid().ToString(), "Properties");
                    allImages.Add(imagePath);
                }
            }

            var dto = _mapper.Map<PropertyDTO>(vm);

            dto.Images = allImages.Select(url => new PropertyImageDTO { ImageUrl = url }).ToList();

            dto.PropertyType = await _propertyTypeService.GetById(vm.PropertyTypeId);
            dto.SalesType = await _saleTypeService.GetById(vm.SalesTypeId);

            await _propertyService.UpdateAsync(dto, dto.Id);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await ValidateUserAsync();
            if (user is null || user.Role != "Agent")
                return RedirectToRoute(new { controller = "Login", action = "Index" });

            var property = await _propertyService.GetById(id);
            if (property is null || property.AgentId != user.Id)
                return RedirectToAction("Index");

            var vm = _mapper.Map<DeletePropertyViewModel>(property);
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            await _propertyService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        private async Task LoadDropdowns(SavePropertyViewModel vm)
        {
            var propertyTypes = await _propertyTypeService.GetAll();
            var salesTypes = await _saleTypeService.GetAll();
            var features = await _featureService.GetAll();

            vm.PropertyTypes = _mapper.Map<List<PropertyTypeViewModel>>(propertyTypes);
            vm.SalesTypes = _mapper.Map<List<SalesTypeViewModel>>(salesTypes);
            vm.AvailableFeatures = _mapper.Map<List<FeatureViewModel>>(features);
        }

    }
}
