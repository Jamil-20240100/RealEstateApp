using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Infrastructure.Identity.Entities;

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

        public ClientHomeController(
            IPropertyService propertyService,
            IFavoriteService favoriteService,
            IMessageService messageService,
            IOfferService offerService,
            UserManager<AppUser> userManager)
        {
            _propertyService = propertyService;
            _favoriteService = favoriteService;
            _messageService = messageService;
            _offerService = offerService;
            _userManager = userManager;
        }

        // ===============================
        // HOME DEL CLIENTE
        // ===============================
        public async Task<IActionResult> Index(PropertyFilterViewModel filter)
        {
            string? userId = _userManager.GetUserId(User);
            var properties = await _propertyService.GetFilteredAvailableAsync(filter, userId);
            ViewBag.Filter = filter; // Para mantener los filtros en la vista
            return View(properties);
        }

        // ===============================
        // DETALLE DE PROPIEDAD
        // ===============================
        public async Task<IActionResult> Details(int id)
        {
            string? clientId = _userManager.GetUserId(User);

            var property = await _propertyService.GetPropertyDetailsAsync(id, clientId);
            if (property == null) return NotFound();

            // Obtener ofertas y mensajes del cliente para esta propiedad
            property.Offers = await _offerService.GetOffersByClientAsync(id, clientId);

            // Determinar el agente asociado (AgentId viene de PropertyViewModel)
            string agentId = property.AgentId;
            property.Messages = await _messageService.GetMessagesAsync(id, clientId, agentId);

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
            string clientId = _userManager.GetUserId(User);
            await _favoriteService.ToggleFavoriteAsync(clientId, propertyId);
            return RedirectToAction(nameof(Index));
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
