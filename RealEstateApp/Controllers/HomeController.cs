using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;

        public HomeController(IAccountServiceForWebApp accountServiceForWebApp)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}