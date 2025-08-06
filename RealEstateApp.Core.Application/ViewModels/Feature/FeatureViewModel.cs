namespace RealEstateApp.Core.Application.ViewModels.Feature
{
    public class FeatureViewModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int NumberOfProperties { get; set; }
    }
}
