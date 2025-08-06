using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.DTOs.User
{
    public class CreateUserRequestDto
    {
        [Required]
        public string Nombre { get; set; } = null!;

        [Required]
        public string Apellido { get; set; } = null!;

        [Required]
        public string Cedula { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = null!;

        [Required]
        public string Usuario { get; set; } = null!;

        [Required]
        public string Contrasena { get; set; } = null!;

        [Required]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = null!;

        [Required]
        public string TipoUsuario { get; set; } = null!;
        public string? UserIdentification { get; set; }
    }
}
