using RealEstateApp.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Identity;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            var email = "admin@mail.com";
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                AppUser admin = new()
                {
                    Name = "Jane",
                    LastName = "Doe",
                    Email = email,
                    EmailConfirmed = true,
                    UserName = "JaneDoeAdmin",
                    IsActive = true,
                    ProfileImage = "",
                    PhoneNumber = "222-222-2222",
                    UserIdentification = "12345678"
                };

                await userManager.CreateAsync(admin, "123Pa$$word!");
                await userManager.AddToRoleAsync(admin, Roles.Admin.ToString());
            }
        }
    }
}