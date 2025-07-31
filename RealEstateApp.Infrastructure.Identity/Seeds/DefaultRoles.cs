using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in new[] { Roles.Admin.ToString(), Roles.Developer.ToString(), Roles.Agent.ToString(), Roles.Client.ToString() })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
