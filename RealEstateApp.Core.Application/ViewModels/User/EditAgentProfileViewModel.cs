using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class EditAgentProfileViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        [Phone(ErrorMessage = "Teléfono inválido")]
        public string PhoneNumber { get; set; } = string.Empty;

        // RUTA relativa (ej: Images/Users/{id}/file.jpg) -> se muestra en la vista
        public string ProfileImageUrl { get; set; } = string.Empty;

        // Archivo subido (opcional)
        public IFormFile? ProfileImageFile { get; set; }
    }
}
