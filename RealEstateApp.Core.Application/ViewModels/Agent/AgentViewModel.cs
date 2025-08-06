using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.ViewModels.Agent
{
    public class AgentViewModel
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Image { get; set; }
        public List<PropertyViewModel>? Properties { get; set; }
    }
}
