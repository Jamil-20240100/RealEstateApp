using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    public class AdminHomeController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;

        public AdminHomeController(IAccountServiceForWebApp accountServiceForWebApp)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}