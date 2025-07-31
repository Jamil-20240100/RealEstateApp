using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.DTOs.User
{
    public class ForgotPasswordWithTokenDto
    {
        [Required]
        public string UserName { get; set; } = null!;
    }

}
