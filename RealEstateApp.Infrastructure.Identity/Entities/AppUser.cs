using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public required string? ProfileImage {  get; set; }
        public required bool IsActive { get; set; }
        public string? UserIdentification { get; set; }
        public override required string? PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }
    }
}
