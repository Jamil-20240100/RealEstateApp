using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class CreateUserViewModel
    {
        public required int Id { get; set; }

        [Required(ErrorMessage = "You must enter the name of user")]
        [DataType(DataType.Text)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "You must enter the last name of user")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "You must enter the email of user")]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "You must enter the username of user")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "You must enter the password of user")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Password must match")]
        [Required(ErrorMessage = "You must enter the confirm password")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must enter the valid role of user")]
        public required string Role { get; set; }

        [Required(ErrorMessage = "You must enter a valid identification")]
        
        public required bool IsActive { get; set; }
        public required string? ProfileImage { get; set; }
        public required string PhoneNumber { get; set; }
        public string? UserIdentification { get; set; }
    }
}
