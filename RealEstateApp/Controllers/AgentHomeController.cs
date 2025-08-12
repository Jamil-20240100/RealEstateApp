using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;


[Authorize(Roles = "Agent")]
public class AgentHomeController : Controller
{
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IPropertyService _propertyService;
    private readonly IMapper _mapper;
    private readonly IOfferService _offerService;
    private readonly UserManager<AppUser> _userManager;

    public AgentHomeController(
        IAccountServiceForWebApp accountServiceForWebApp,
        IPropertyService propertyService,
        IOfferService offerService,
        IMapper mapper,
        UserManager<AppUser> userManager)
    {
        _accountServiceForWebApp = accountServiceForWebApp;
        _propertyService = propertyService;
        _mapper = mapper;
        _offerService = offerService;
        _userManager = userManager;
    }
    private async Task<UserViewModel?> ValidateUserAsync()
    {
        var userSession = await _userManager.GetUserAsync(User);
        if (userSession == null) return null;

        var user = await _accountServiceForWebApp.GetUserByUserName(userSession.UserName ?? "");
        if (user == null || user.Role != Roles.Agent.ToString()) return null;

        return _mapper.Map<UserViewModel>(user);
    }

    public async Task<IActionResult> Index()
    {
        
        var userName = User.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            return RedirectToRoute(new { controller = "Login", action = "Index" });

        var user = await ValidateUserAsync();
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
        var user = await ValidateUserAsync();
        if (user == null)
            return RedirectToRoute(new { controller = "Login", action = "Index" });


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
        var user = await ValidateUserAsync();
        if (user == null)
            return RedirectToRoute(new { controller = "Login", action = "Index" });

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
