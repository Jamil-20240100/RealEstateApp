using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Controllers
{
    public class AgentHomeController : Controller
    {
        private readonly IAccountServiceForWebApp _accountServiceForWebApp;

        public AgentHomeController(IAccountServiceForWebApp accountServiceForWebApp)
        {
            _accountServiceForWebApp = accountServiceForWebApp;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
