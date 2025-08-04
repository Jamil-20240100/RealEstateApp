using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Seeders
{
    public static class DefaultPropertiesSeeder
    {
        public static async Task SeedAsync(RealEstateContext context)
        {
            if (!context.Properties.Any())  // Solo insertamos si no existen propiedades
            {
                var properties = GenerateProperties(context);  // Pasar el contexto para obtener los Ids dinámicamente
                await context.Properties.AddRangeAsync(properties);
                await context.SaveChangesAsync();
            }
        }

        private static List<Property> GenerateProperties(RealEstateContext context)
        {
            var properties = new List<Property>();

            // Obtener el PropertyTypeId de la tabla PropertyTypes
            var propertyTypeId = context.PropertyTypes
                .Where(pt => pt.Name == "Casa")  // Puedes cambiar "Casa" por el tipo que prefieras
                .Select(pt => pt.Id)
                .FirstOrDefault();

            if (propertyTypeId == 0)
            {
                throw new Exception("No se encontró el tipo de propiedad 'Casa'.");
            }

            // Lista de 50 imágenes únicas
            var images = new List<string>();
            for (int i = 1; i <= 30; i++)
            {
                images.Add($"Images/property{i}.jpg");
            }

            for (int i = 1; i <= 30; i++)
            {
                properties.Add(new Property
                {
                    Code = $"PROP{i:00}",
                    Price = new decimal(50000 + (i * 1000)),
                    Size = new Random().Next(50, 200),
                    Bathrooms = new Random().Next(1, 3),
                    Bedrooms = new Random().Next(1, 5),
                    Description = $"Descripción de la propiedad {i}",
                    AgentId = "agent1", // Aquí puedes agregar el ID del agente que prefieras
                    State = "Disponible", // Estado "Disponible"
                    PropertyTypeId = propertyTypeId, // Usar el PropertyTypeId dinámico
                    SaleTypeId = 1, // Ajusta según tus tipos de venta
                                    // Asignar una imagen única para cada propiedad
                    Images = new List<PropertyImage>
            {
                new PropertyImage { ImageUrl = images[i - 1] }  // Usar la imagen correspondiente
            }
                });
            }

            return properties;
        }

    }
}
