using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds
{
    public static class DefaultClientUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            AppUser user = new()
            {
                Name = "Peter",
                LastName = "Doe",
                Email = "client@mail.com",
                EmailConfirmed = true,
                UserName = "PeterDoeClient",
                IsActive = true,
                ProfileImage = "",
                PhoneNumber = "111-111-1111"
            };

            if (await userManager.Users.AllAsync(u => u.Id != user.Id))
            {
                var entityUser = await userManager.FindByEmailAsync(user.Email);
                if (entityUser == null)
                {
                    await userManager.CreateAsync(user, "123Pa$$word!");
                    await userManager.AddToRoleAsync(user, Roles.Client.ToString());
                }
            }

        }
    }
}
