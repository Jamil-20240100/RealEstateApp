using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Core.Application.DTOs.Email;

namespace RealEstateApp.Infrastructure.Shared.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendAsync(EmailRequestDto emailRequestDto)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(emailRequestDto.To))
                {
                    emailRequestDto.ToRange ??= new List<string>();
                    if (!emailRequestDto.ToRange.Contains(emailRequestDto.To))
                        emailRequestDto.ToRange.Add(emailRequestDto.To);
                }

                var email = new MimeMessage
                {
                    Sender = MailboxAddress.Parse(_mailSettings.EmailFrom),
                    Subject = emailRequestDto.Subject
                };

                foreach (var toItem in emailRequestDto.ToRange ?? Enumerable.Empty<string>())
                {
                    email.To.Add(MailboxAddress.Parse(toItem));
                }

                var builder = new BodyBuilder
                {
                    HtmlBody = emailRequestDto.HtmlBody
                };

                email.Body = builder.ToMessageBody();

                using var smtpClient = new SmtpClient();
                // Consider adding a timeout for connect and send operations
                await smtpClient.ConnectAsync(_mailSettings.SmtpHost, _mailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(_mailSettings.SmtpUser, _mailSettings.SmtpPass);
                await smtpClient.SendAsync(email);
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient} with subject '{Subject}'.",
                                       string.Join(", ", emailRequestDto.ToRange ?? new List<string> { emailRequestDto.To }),
                                       emailRequestDto.Subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while sending an email to {Recipient} with subject '{Subject}'.",
                                   string.Join(", ", emailRequestDto.ToRange ?? new List<string> { emailRequestDto.To }),
                                   emailRequestDto.Subject);
                // Optionally re-throw a more user-friendly exception or handle it
                throw new InvalidOperationException("Failed to send email notification.", ex);
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailRequest = new EmailRequestDto
            {
                To = toEmail,
                Subject = subject,
                HtmlBody = $"<p>{message.Replace("\n", "<br>")}</p>"
            };

            await SendAsync(emailRequest);
        }
    }
}