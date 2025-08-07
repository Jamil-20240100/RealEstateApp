using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Seeders
{
    public static class DefaultPropertiesSeeder
    {
        public static async Task SeedAsync(RealEstateContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await context.Properties.AnyAsync())
            {
                // Obtener nombre del rol "Agent" desde el enum
                var agentRoleName = Roles.Agent.ToString();

                // Verifica que exista el rol
                var agentRole = await roleManager.FindByNameAsync(agentRoleName);
                if (agentRole == null)
                    throw new Exception($"No se encontró el rol '{agentRoleName}'.");

                // Obtener usuarios con rol "Agent"
                var agents = await userManager.GetUsersInRoleAsync(agentRoleName);
                var agent = agents.FirstOrDefault();

                if (agent == null)
                    throw new Exception($"No se encontró ningún usuario con rol '{agentRoleName}'.");

                var properties = await GeneratePropertiesAsync(context, userManager, agent);
                await context.Properties.AddRangeAsync(properties);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<List<Property>> GeneratePropertiesAsync(RealEstateContext context, UserManager<AppUser> userManager, AppUser agent)
        {
            var properties = new List<Property>();
            var random = new Random();

            var propertyTypeId = await context.PropertyTypes
                .Where(pt => pt.Name == "Casa")
                .Select(pt => pt.Id)
                .FirstOrDefaultAsync();

            if (propertyTypeId == 0)
                throw new Exception("No se encontró el tipo de propiedad 'Casa'.");

            var salesTypeId = await context.SalesTypes
                .Where(st => st.Name == "Venta")
                .Select(st => st.Id)
                .FirstOrDefaultAsync();

            if (salesTypeId == 0)
                throw new Exception("No se encontró el tipo de venta 'Venta'.");

            var images = Enumerable.Range(1, 30)
                                   .Select(i => $"Images/property{i}.jpg")
                                   .ToList();

            for (int i = 1; i <= 30; i++)
            {
                var property = new Property
                {
                    Id = 0,
                    AgentId = agent.Id,
                    Price = 50000 + (i * 1000),
                    Description = $"Propiedad {i}: Casa familiar con excelente ubicación.",
                    SizeInMeters = random.Next(50, 200),
                    NumberOfBathrooms = random.Next(1, 3),
                    NumberOfRooms = random.Next(1, 5),
                    PropertyTypeId = propertyTypeId,
                    SalesTypeId = salesTypeId,
                    Features = new List<Feature>(),
                    Images = new List<PropertyImage>
                    {
                        new PropertyImage { ImageUrl = images[i - 1] }
                    },
                    State = PropertyState.Disponible
                };

                properties.Add(property);
            }

            return properties;
        }
    }
}
