using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstateApp.Infrastructure.Persistence.Seeders
{
    public static class DefaultPropertyTypeAndSaleTypeSeeder
    {
        public static async Task SeedPropertyTypesAndSaleTypesAsync(RealEstateContext context)
        {
            // Seed Property Types
            if (!context.PropertyTypes.Any())
            {
                var propertyTypes = new List<PropertyType>
        {
            new PropertyType { Name = "Casa", Description = "Casa residencial con jardín y cochera." },
            new PropertyType { Name = "Apartamento", Description = "Apartamento de varios pisos en un edificio." },
            new PropertyType { Name = "Terreno", Description = "Terreno vacío para construcción o cultivo." },
            new PropertyType { Name = "Oficina", Description = "Espacio comercial para oficinas." },
            new PropertyType { Name = "Local Comercial", Description = "Local comercial para venta o alquiler." }
        };

                await context.PropertyTypes.AddRangeAsync(propertyTypes);
                await context.SaveChangesAsync();
            }

            // Seed Sale Types
            if (!context.SaleTypes.Any())
            {
                var saleTypes = new List<SaleType>
        {
            new SaleType { Name = "Venta", Description = "Transacción de compra-venta de una propiedad." },
            new SaleType { Name = "Alquiler", Description = "Acuerdo de arrendamiento de una propiedad." }
        };

                await context.SaleTypes.AddRangeAsync(saleTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
