using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.ViewModels.User;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Helpers; // si pones FileManager aquí
using System.Threading.Tasks;

[Authorize(Roles = "Agent")]
public class MyProfileController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public MyProfileController(UserManager<AppUser> userManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Login");

        var vm = new EditAgentProfileViewModel
        {
            Name = user.Name,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImage) ? "/Images/default-user.png" : "/" + user.ProfileImage
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(EditAgentProfileViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Index", "Login");

        if (!ModelState.IsValid)
        {
            // mantener la url de la imagen actual en caso de validación fallida
            vm.ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImage) ? "/Images/default-user.png" : "/" + user.ProfileImage;
            return View(vm);
        }

        // Actualizar campos
        user.Name = vm.Name;
        user.LastName = vm.LastName;
        user.PhoneNumber = vm.PhoneNumber;

        // Si subieron una imagen, guardarla y asignar ruta en user.ProfileImage
        if (vm.ProfileImageFile != null && vm.ProfileImageFile.Length > 0)
        {
            // Opción A: si tienes un FileManager.Upload(file, name, folder) existente (como lo usaste en Property)
            var uploadedPath = FileManager.Upload(vm.ProfileImageFile, user.Id, "Users"); // tu implementación puede variar

            
            user.ProfileImage = uploadedPath; // guarda ruta relativa, ej: Images/Users/{userId}/{file}
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            vm.ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImage) ? "/Images/default-user.png" : "/" + user.ProfileImage;
            return View(vm);
        }

        TempData["Success"] = "Perfil actualizado correctamente.";
        return RedirectToAction("Index", "AgentHome");
    }

    // Helper local para guardar la imagen en wwwroot/Images/Users/{userId}/filename.ext y devolver ruta relativa
    private async Task<string> SaveUserImageAsync(IFormFile file, string userId)
    {
        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{ext}";
        var folder = Path.Combine(_env.WebRootPath, "Images", "Users", userId);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Devuelve ruta relativa sin prefijo "~/" ni "/"
        return $"Images/Users/{userId}/{fileName}";
    }
}
