using RealEstateApp.Core.Application.DTOs.Client;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.DTOs.Offer;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;

namespace RealEstateApp.Core.Application.DTOs.Property
{
    public class PropertyDTO
    {
        public required int Id { get; set; }
        public required PropertyTypeDTO PropertyType { get; set; }
        public required SalesTypeDTO SalesType { get; set; }
        public required decimal Price { get; set; }
        public required string Description { get; set; }
        public required decimal SizeInMeters { get; set; }
        public required int NumberOfRooms { get; set; }
        public required int NumberOfBathrooms { get; set; }
        public required List<FeatureDTO> Features { get; set; }
        public List<PropertyImageDTO> Images { get; set; } = new List<PropertyImageDTO>();
        public string? AgentId { get; set; }
        public bool IsFavorite { get; set; }

        public List<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
        public List<OfferDTO> Offers { get; set; } = new List<OfferDTO>();
        public List<ClientDTO> ClientsWithOffers { get; set; } = new List<ClientDTO>();
    }
}
