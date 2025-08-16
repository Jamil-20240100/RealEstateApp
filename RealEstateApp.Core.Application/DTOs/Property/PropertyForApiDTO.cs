using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.DTOs.Property
{
    public class PropertyForApiDTO
    {
        public  int Id { get; set; }
        public  PropertyTypeDTO PropertyType { get; set; }
        public  SalesTypeDTO SalesType { get; set; }
        public  decimal Price { get; set; }
        public  string Description { get; set; }
        public  decimal SizeInMeters { get; set; }
        public  int NumberOfRooms { get; set; }
        public  int NumberOfBathrooms { get; set; }
        public  List<FeatureDTO> Features { get; set; }
        public  string AgentId { get; set; }
        public  string AgentName { get; set; }
        public  string Code { get; set; }
        public PropertyState State { get; set; }
    }
}
