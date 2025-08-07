namespace RealEstateApp.Core.Domain.Entities
{
    public class Feature
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required ICollection<Property>? Properties { get; set; }
    }
}