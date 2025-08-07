namespace RealEstateApp.Core.Application.ViewModels.PropertyType
{
    public class PropertyTypeViewModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int NumberOfProperties { get; set; }
    }
}
