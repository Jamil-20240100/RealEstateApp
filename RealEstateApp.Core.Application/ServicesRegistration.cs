using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Interfaces;
using System.Reflection;

namespace RealEstateApp.Core.Application
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            #region Configurations
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            #endregion

            #region Services IOC

            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<ISalesTypeService, SalesTypeService>();
            services.AddScoped<IFeatureService, FeatureService>();

            services.AddScoped<IMessageService, MessageService>();
            #endregion
        }
    }
}
