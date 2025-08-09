using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyViewModel
    {
        public required int Id { get; set; }
        public required PropertyTypeViewModel PropertyType { get; set; }
        public required SalesTypeViewModel SalesType { get; set; }
        public required decimal Price { get; set; }
        public required string Code { get; set; }
        public required string Description { get; set; }
        public required decimal SizeInMeters { get; set; }
        public required int NumberOfRooms { get; set; }
        public required int NumberOfBathrooms { get; set; }
        public required List<FeatureViewModel> Features { get; set; }
        public List<string>? Images { get; set; }
        public bool IsBoughtByCurrentClient { get; set; }
        public string? BuyerClientId { get; set; }
        public PropertyState State { get; set; }
        public bool IsSold { get; set; }
    }
}
