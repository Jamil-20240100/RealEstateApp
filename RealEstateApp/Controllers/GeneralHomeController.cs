using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;


namespace RealEstateApp.Controllers
{
    public class GeneralHomeController : Controller
    {
        private readonly IPropertyService _propertyService;
        public GeneralHomeController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        // ===============================
        // HOME GENERAL
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Index(PropertyFilterViewModel filter)
        {
            // En el home general no hay usuario logueado, por eso enviamos null
            var properties = await _propertyService.GetFilteredAvailableAsync(filter, null);
            ViewBag.Filter = filter; // para mantener filtros en la vista
            return View(properties);
        }

        // ===============================
        // DETALLE DE PROPIEDAD
        // ===============================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var property = await _propertyService.GetPropertyDetailsAsync(id, null);
            if (property == null) return NotFound();

            return View(property);
        }
    }
}