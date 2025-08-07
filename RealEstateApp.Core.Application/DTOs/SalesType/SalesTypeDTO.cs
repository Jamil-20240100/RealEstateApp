namespace RealEstateApp.Core.Application.DTOs.SalesType
{
    public class SalesTypeDTO
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int NumberOfProperties { get; set; }
    }
}
