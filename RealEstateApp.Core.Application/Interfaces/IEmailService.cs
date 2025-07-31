using RealEstateApp.Core.Application.DTOs.Email;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}