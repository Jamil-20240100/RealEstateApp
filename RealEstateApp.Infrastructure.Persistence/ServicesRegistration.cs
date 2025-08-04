using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using RealEstateApp.Infrastructure.Persistence.Seeders;
using RealEstateApp.Infrastructure.Seeders;

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
                services.AddScoped<IFavoritePropertyRepository, FavoritePropertyRepository>();
                services.AddScoped<IMessageRepository, MessageRepository>();
                services.AddScoped<IOfferRepository, OfferRepository>();
                #endregion
            }
        }
        public static async Task RunPersistenceSeedAsync(this IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var servicesProvider = scope.ServiceProvider;

            // Obtener el contexto de base de datos
            var context = servicesProvider.GetRequiredService<RealEstateContext>();

            // Primero, insertar los tipos de propiedad y venta
            await DefaultPropertyTypeAndSaleTypeSeeder.SeedPropertyTypesAndSaleTypesAsync(context);

            // Verificar si los SaleTypes se insertaron correctamente
            if (!context.SaleTypes.Any())
            {
                throw new Exception("Los SaleTypes no se insertaron correctamente.");
            }

            // Luego, insertar las propiedades, una vez que los tipos de propiedad existen
            await DefaultPropertiesSeeder.SeedAsync(context);
        }
    }
}