using Microsoft.AspNetCore.Mvc;

namespace RealEstateApp.Controllers
{
    public class GeneralHomeController : Controller
    {
        public GeneralHomeController()
        {
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}