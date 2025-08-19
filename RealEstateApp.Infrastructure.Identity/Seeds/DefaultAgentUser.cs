using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAgentUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            /*
            var email = "agent@mail.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user != null)
            {
                AppUser admin = new()
                {
                    Name = "Lisa",
                    LastName = "Doe",
                    Email = email,
                    EmailConfirmed = true,
                    UserName = "LisaDoeAgent",
                    IsActive = true,
                    ProfileImage = "Images/Users/0c408629-39bd-457c-81ff-923926de73ab",
                    PhoneNumber = "111-111-1111",
                };

                await userManager.CreateAsync(admin, "123Pa$$word!");
                await userManager.AddToRoleAsync(admin, Roles.Agent.ToString());
            }
            */
        }
    }
}