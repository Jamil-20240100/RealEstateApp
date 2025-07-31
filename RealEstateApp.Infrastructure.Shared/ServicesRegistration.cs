using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Settings;
using RealEstateApp.Infrastructure.Shared.Services;

namespace RealEstateApp.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddSharedLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Configurations
            services.Configure<MailSettings>(config.GetSection("MailSettings"));
            #endregion

            #region Services IOC
            services.AddScoped<IEmailService, EmailService>();
            #endregion
        }
    }
}