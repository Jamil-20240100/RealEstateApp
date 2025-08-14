using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.ViewModels.Property
{
   
        public class PropertyDetailsViewModel
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

            // Nuevas propiedades para la vista de detalles del agente
            public List<OfferViewModel> Offers { get; set; } = new();
            public List<MessageViewModel> Messages { get; set; } = new();
            public List<ClientsWhoMadeOfferViewModel> ClientsWithOffers { get; set; } = new();
            public List<ClientsWhoMadeOfferViewModel> ClientsWhoMessaged { get; set; } = new();

            public string? AgentId { get; set; }
            public string? SelectedClientId { get; set; }
        public IAsyncEnumerable<char>? AgentName { get; set; }
    }
    
}
