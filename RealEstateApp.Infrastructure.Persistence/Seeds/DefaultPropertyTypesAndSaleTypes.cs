using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Seeders
{
    public static class DefaultPropertyTypeAndSalesTypeSeeder
    {
        public static async Task SeedPropertyTypesAndSaleTypesAsync(RealEstateContext context)
        {
            // Seed Property Types
            if (!context.PropertyTypes.Any())
            {
                var propertyTypes = new List<PropertyType>
                {
                    new PropertyType { Id = 0, Name = "Casa", Description = "Casa residencial con jardín y cochera.", Properties = new List<Property>() },
                    new PropertyType { Id = 0, Name = "Apartamento", Description = "Apartamento de varios pisos en un edificio.", Properties = new List<Property>() },
                    new PropertyType { Id = 0, Name = "Terreno", Description = "Terreno vacío para construcción o cultivo.", Properties = new List<Property>() },
                    new PropertyType { Id = 0, Name = "Oficina", Description = "Espacio comercial para oficinas.", Properties = new List<Property>() },
                    new PropertyType { Id = 0, Name = "Local Comercial", Description = "Local comercial para venta o alquiler.", Properties = new List<Property>() }
                };

                await context.PropertyTypes.AddRangeAsync(propertyTypes);
                await context.SaveChangesAsync();
            }

            // Seed Sales Types
            if (!context.SalesTypes.Any())
            {
                var salesTypes = new List<SalesType>
                {
                    new SalesType { Id = 0, Name = "Venta", Description = "Transacción de compra-venta de una propiedad.", Properties = new List<Property>() },
                    new SalesType { Id = 0, Name = "Alquiler", Description = "Acuerdo de arrendamiento de una propiedad.", Properties = new List<Property>() }
                };

                await context.SalesTypes.AddRangeAsync(salesTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
