namespace RealEstateApp.Core.Domain.Entities
{
    public class Property
    {
        public required int Id { get; set; }
        public required string AgentId { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required decimal SizeInMeters { get; set; }
        public required int NumberOfRooms { get; set; }
        public required int NumberOfBathrooms { get; set; }
        public required ICollection<Feature> Features { get; set; }
        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
        public int PropertyTypeId { get; set; }
        public PropertyType? PropertyType { get; set; }
        public int SalesTypeId { get; set; }
        public SalesType? SalesType { get; set; }
        public required string Code { get; set; }
    }
}