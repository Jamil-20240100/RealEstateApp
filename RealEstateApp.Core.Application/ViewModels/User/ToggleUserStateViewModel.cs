namespace RealEstateApp.Core.Application.ViewModels.User
{
    public class ToggleUserStateViewModel
    {
        public required string Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public required bool IsActive { get; set; }
        public required string? ProfileImage { get; set; }
        public required string PhoneNumber { get; set; }
    }
}