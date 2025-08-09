using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;

public class AgentHomeController : Controller
{
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IPropertyService _propertyService;
    private readonly IMapper _mapper;
    private readonly IOfferService _offerService;

    public AgentHomeController(
        IAccountServiceForWebApp accountServiceForWebApp,
        IPropertyService propertyService,
        IOfferService offerService,
        IMapper mapper)
    {
        _accountServiceForWebApp = accountServiceForWebApp;
        _propertyService = propertyService;
        _mapper = mapper;
        _offerService = offerService;
    }

    public async Task<IActionResult> Index()
    {
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return RedirectToRoute(new { controller = "Login", action = "Index" });

        var user = await _accountServiceForWebApp.GetUserByUserName(userName);
        if (user == null)
            return RedirectToRoute(new { controller = "Login", action = "Index" });

        var propertiesDto = await _propertyService.GetAllWithInclude();

        var agentPropertiesDto = propertiesDto
            .Where(p => p.AgentId == user.Id)
            .ToList();

        var agentPropertiesVm = _mapper.Map<List<PropertyViewModel>>(agentPropertiesDto);

        return View(agentPropertiesVm);
    }

    public async Task<IActionResult> Details(int id, string selectedClientId = null)
    {
        var property = await _propertyService.GetPropertyDetailsAsync(id, null);

        if (property == null)
        {
            return NotFound();
        }

        property.SelectedClientId = selectedClientId;

        return RedirectToAction("Details", "Property", new { id = id, selectedClientId = selectedClientId });
    }

    [HttpPost]
    public async Task<IActionResult> RespondToOffer(int offerId, bool isAccepted)
    {
        try
        {
            await _offerService.RespondToOfferAsync(offerId, isAccepted);
            // Puedes redirigir a la lista de propiedades o a la página de detalles de la propiedad.
            // Por ejemplo, si tienes el ID de la propiedad, úsalo para redirigir a detalles.
            // Si no, redirige a un lugar general:
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            // Opcional: manejar error y mostrar mensaje o redirigir con error
            return BadRequest(ex.Message);
        }
    }


}
