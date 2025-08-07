using RealEstateApp.Core.Application.DTOs.Property;

namespace RealEstateApp.Core.Application.DTOs.Agent
{
    public class AgentDTO
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public required string Image { get; set; }
        public List<PropertyDTO>? Properties { get; set; }
    }
}
