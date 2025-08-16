using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
//using RealEstateApp.Infrastructure.Persistence.Seeders;

namespace RealEstateApp.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<RealEstateContext>(opt =>
                                              opt.UseInMemoryDatabase("AppDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<RealEstateContext>(
                    (serviceProvider, opt) =>
                    {
                        opt.EnableSensitiveDataLogging();
                        opt.UseSqlServer(connectionString,
                        m => m.MigrationsAssembly(typeof(RealEstateContext).Assembly.FullName));
                    },
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped
                    );
                #endregion

                #region repositories IOC
                services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
              
                services.AddScoped<IPropertyRepository, PropertyRepository>();
                services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
                services.AddScoped<ISalesTypeRepository, SalesTypeRepository>();
                services.AddScoped<IFeatureRepository, FeatureRepository>();
                services.AddScoped<IFavoritePropertyRepository, FavoritePropertyRepository>();
                services.AddScoped<IMessageRepository, MessageRepository>();
                services.AddScoped<IOfferRepository, OfferRepository>();
                #endregion
            }
        }
    }
}