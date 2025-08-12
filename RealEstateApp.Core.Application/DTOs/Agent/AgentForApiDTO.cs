namespace RealEstateApp.Core.Application.DTOs.Agent
{
    public class AgentForApiDTO
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public int NumberOfProperties { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
