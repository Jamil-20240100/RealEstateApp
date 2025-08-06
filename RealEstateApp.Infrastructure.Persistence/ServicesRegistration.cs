using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using RealEstateApp.Infrastructure.Persistence.Seeders;

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

        public static async Task RunPersistenceSeedAsync(this IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var sp = scope.ServiceProvider;

            var context = sp.GetRequiredService<RealEstateContext>();

            // 1) Asegurar DB/migraciones
            //    (Si prefieres EnsureCreated para escenarios de demo: usa context.Database.EnsureCreatedAsync())
            await context.Database.MigrateAsync();

            // 2) Transacción para garantizar consistencia
            using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                // 3) Semilla de tipos de propiedad y tipos de venta (idempotente)
                await DefaultPropertyTypeAndSalesTypeSeeder.SeedPropertyTypesAndSaleTypesAsync(context);

                // 4) Verificación (evita NullReference y confirma que realmente hay datos)
                var saleTypesCount = await context.SalesTypes.CountAsync();
                if (saleTypesCount == 0)
                    throw new InvalidOperationException("Los SaleTypes no se insertaron correctamente.");

                var propertyTypesCount = await context.PropertyTypes.CountAsync();
                if (propertyTypesCount == 0)
                    throw new InvalidOperationException("Los PropertyTypes no se insertaron correctamente.");

                // 5) Semilla de propiedades (usa los tipos existentes)
                var userManager = sp.GetRequiredService<UserManager<AppUser>>();
                var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

                await DefaultPropertiesSeeder.SeedAsync(context, userManager, roleManager);

                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}