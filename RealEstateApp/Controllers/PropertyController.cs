using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Helpers;
using RealEstateApp.Infrastructure.Identity.Entities;

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
        private readonly IMessageService _messageService;
        private readonly IOfferService _offerService;

        public PropertyController(
            IPropertyService propertyService,
            IPropertyTypeService propertyTypeService,
            ISalesTypeService saleTypeService,
            IFeatureService featureService,
            IAccountServiceForWebApp userService,
            IMapper mapper, UserManager<AppUser> userManager,
            IMessageService messageService,
            IOfferService offerService
        )
        {
            _propertyService = propertyService;
            _propertyTypeService = propertyTypeService;
            _saleTypeService = saleTypeService;
            _featureService = featureService;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
            _messageService = messageService;
            _offerService = offerService;
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

        [HttpGet]
        public async Task<IActionResult> Details(int id, string? selectedClientId = null)
        {
            string agentId = _userManager.GetUserId(User);

            // Validación: solo el agente dueño de la propiedad puede verla
            var agentCheck = await _propertyService.GetAgentIdByPropertyIdAsync(id);
            if (agentCheck != agentId)
                return Forbid();

            // Obtener datos base de la propiedad
            var property = await _propertyService.GetPropertyDetailsAsync(id, agentId);
            if (property == null) return NotFound();

            // Obtener todos los clientes que enviaron mensajes al agente sobre esta propiedad
            var clientsWhoMessagedIds = await _messageService.GetClientsWhoMessagedAgentForPropertyAsync(agentId, id);

            var clientsInfo = await _userManager.Users
            .Where(u => clientsWhoMessagedIds.Contains(u.Id))
            .Select(u => new ClientsWhoMadeOfferViewModel
            {
                Id = u.Id,
                FullName = u.UserName, // <-- Usamos UserName
                ProfileImageUrl = "~/Images/default-user.png", // <-- Imagen por defecto
                OfferAmount = 0
            })
            .ToListAsync();

            property.ClientsWhoMessaged = clientsInfo;

            // Obtener todos los mensajes de la propiedad
            var messages = await _messageService.GetMessagesAsync(id, null, agentId);

            // Si hay un cliente seleccionado (vía querystring o click), filtra su chat
            if (!string.IsNullOrEmpty(selectedClientId))
            {
                property.SelectedClientId = selectedClientId;
                property.Messages = messages
                    .Where(m => m.SenderId == selectedClientId || m.ReceiverId == selectedClientId)
                    .OrderBy(m => m.SentAt)
                    .ToList();
            }
            else
            {
                // Si no hay cliente seleccionado, no cargamos mensajes
                property.Messages = new List<MessageViewModel>();
            }

            // Obtener todas las ofertas
            property.Offers = await _offerService.GetOffersForPropertyAsync(id);

            return View(property);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageViewModel vm)
        {
            string senderId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(vm.Content))
                return RedirectToAction(nameof(Details), new { id = vm.PropertyId });

            await _messageService.SendMessageAsync(senderId, vm.ReceiverId, vm.PropertyId, vm.Content, isFromClient: false);

            return RedirectToAction(nameof(Details), new { id = vm.PropertyId });
        }

        [HttpPost]
        public async Task<IActionResult> AcceptOffer(int offerId)
        {
            await _offerService.AcceptOfferAsync(offerId);
            return RedirectToAction(nameof(Details), new { id = await _offerService.GetPropertyIdByOfferAsync(offerId) });
        }

        [HttpPost]
        public async Task<IActionResult> RejectOffer(int offerId)
        {
            await _offerService.RejectOfferAsync(offerId);
            return RedirectToAction(nameof(Details), new { id = await _offerService.GetPropertyIdByOfferAsync(offerId) });
        }

    }
}
