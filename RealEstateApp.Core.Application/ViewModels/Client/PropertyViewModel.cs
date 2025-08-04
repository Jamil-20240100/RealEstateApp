namespace RealEstateApp.Core.Application.ViewModels.Client
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }         // Nombre de PropertyType
        public string SaleType { get; set; }     // Nombre de SaleType
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double Size { get; set; }
        public string Description { get; set; }
        public string AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; }
        public string AgentPhoto { get; set; }

        // Slider de imágenes
        public List<string> Images { get; set; } = new();

        // Mejoras
        public List<string> Improvements { get; set; } = new();

        // Favoritos
        public bool IsFavorite { get; set; }

        // Chat y ofertas
        public List<MessageViewModel> Messages { get; set; } = new();
        public List<OfferViewModel> Offers { get; set; } = new();
    }
}
