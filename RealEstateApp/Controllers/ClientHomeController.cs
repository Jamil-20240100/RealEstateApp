using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.Security.Claims;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientHomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IFavoriteService _favoriteService;
        private readonly IMessageService _messageService;
        private readonly IOfferService _offerService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public ClientHomeController(
            IPropertyService propertyService,
            IFavoriteService favoriteService,
            IMessageService messageService,
            IOfferService offerService,
            IMapper mapper,
            UserManager<AppUser> userManager)
        {
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _messageService = messageService;
            _offerService = offerService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // ===============================
        // HOME DEL CLIENTE
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Esto devuelve DTOs
            var dtoList = await _propertyService.GetAllWithInclude();

            // Mapear a ViewModels para la vista
            var vmList = _mapper.Map<List<PropertyViewModel>>(dtoList);

            // Cargar favoritos y pasarlos como HashSet<int>
            HashSet<int> favoriteIds = new();
            if (!string.IsNullOrEmpty(userId))
            {
                var favVms = await _favoriteService.GetFavoritePropertiesByUserAsync(userId);
                favoriteIds = favVms.Select(f => f.Id).ToHashSet();
            }

            ViewBag.FavoriteIds = favoriteIds;

            return View(vmList); // <- la vista espera List<PropertyViewModel>
        }


        // ===============================
        // DETALLE DE PROPIEDAD
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            // Obtener la propiedad sin validar que el usuario sea agente dueño
            var propertyDto = await _propertyService.GetByIdWithInclude(id);
            if (propertyDto == null)
                return NotFound();

            // Mapear DTO a ViewModel (ajusta según tus modelos)
            var property = _mapper.Map<PropertyDetailsViewModel>(propertyDto);

            // Opcional: si quieres cargar mensajes y ofertas para cliente
            string clientId = _userManager.GetUserId(User);
            string agentId = await _propertyService.GetAgentIdByPropertyIdAsync(id);

            // Mensajes donde el cliente es parte (ya sea remitente o receptor)
            var messages = await _messageService.GetMessagesAsync(id, clientId, agentId);
            property.Messages = messages;

            // Ofertas hechas por el cliente para esa propiedad
            var offers = await _offerService.GetOffersByClientAsync(id, clientId);
            property.Offers = offers;

            // No hay restricción estricta, clientes pueden ver cualquier propiedad

            return View(property);
        }
            

        // ===============================
        // MIS PROPIEDADES (FAVORITOS)
        // ===============================
        public async Task<IActionResult> MyProperties()
        {
            string clientId = _userManager.GetUserId(User);
            var favorites = await _favoriteService.GetFavoritePropertiesByUserAsync(clientId);
            return View(favorites);
        }

        // ===============================
        // MARCAR / DESMARCAR FAVORITO
        // ===============================
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int propertyId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            await _favoriteService.ToggleFavoriteAsync(userId, propertyId);

            // Redirige a la lista de propiedades para que se recargue con datos actualizados
            return RedirectToAction("Index");
        }

        // ===============================
        // ENVIAR MENSAJE (CHAT)
        // ===============================
        [HttpPost]
        public async Task<IActionResult> SendMessage(CreateMessageViewModel vm)
        {
            string senderId = _userManager.GetUserId(User);

            if (string.IsNullOrWhiteSpace(vm.Content))
                return RedirectToAction(nameof(Details), new { id = vm.PropertyId });

            await _messageService.SendMessageAsync(senderId, vm.ReceiverId, vm.PropertyId, vm.Content, isFromClient: true);

            return RedirectToAction(nameof(Details), new { id = vm.PropertyId });
        }

        // ===============================
        // CREAR OFERTA
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CreateOffer(CreateOfferViewModel vm)
        {
            string clientId = _userManager.GetUserId(User);

            if (vm.Amount <= 0)
            {
                TempData["OfferError"] = "La oferta debe ser mayor a 0.";
                return RedirectToAction(nameof(Details), new { id = vm.PropertyId });
            }

            await _offerService.CreateOfferAsync(clientId, vm.PropertyId, vm.Amount);

            return RedirectToAction(nameof(Details), new { id = vm.PropertyId });
        }
    }
}
