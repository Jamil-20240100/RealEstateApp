using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultDeveloperUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            var email = "developer@mail.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                AppUser admin = new()
                {
                    Name = "Bob",
                    LastName = "Doe",
                    Email = email,
                    EmailConfirmed = true,
                    UserName = "BobDoeDeveloper",
                    IsActive = true,
                    ProfileImage = "",
                    PhoneNumber = "111-111-1111"
                };

                await userManager.CreateAsync(admin, "123Pa$$word!");
                await userManager.AddToRoleAsync(admin, Roles.Developer.ToString());
            }
        }
    }
}