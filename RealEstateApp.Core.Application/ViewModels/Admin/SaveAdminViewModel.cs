using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Admin
{
    public class SaveAdminViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Debes ingresar el nombre.")]
        [DataType(DataType.Text)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes ingresar el apellido.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes ingresar la cédula.")]
        [DataType(DataType.Text)]
        public string UserIdentification { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes ingresar el correo.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes ingresar el nombre de usuario.")]
        [DataType(DataType.Text)]
        public string UserName { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}