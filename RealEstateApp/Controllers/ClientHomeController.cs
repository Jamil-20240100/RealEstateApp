using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientHomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ClientHomeController(
            UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}