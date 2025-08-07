using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
    public class PropertyFilterViewModel
    {
        public string? CodeContains { get; set; }
        public PropertyTypeViewModel? PropertyType { get; set; }
        public SalesTypeViewModel? SalesType { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinSizeInMeters { get; set; }
        public decimal? MaxSizeInMeters { get; set; }
        public int? MinNumberOfRooms { get; set; }
        public int? MinNumberOfBathrooms { get; set; }
        public List<int>? FeatureIds { get; set; }
        public bool OnlyFavorites { get; set; } = false;
    }
}
