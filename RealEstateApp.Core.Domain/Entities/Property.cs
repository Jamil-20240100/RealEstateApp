using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int PropertyTypeId { get; set; }
        public int SaleTypeId { get; set; }
        public decimal Price { get; set; }
        public double Size { get; set; }
        public int Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public string Description { get; set; }
        public string AgentId { get; set; }
        public string State { get; set; } = "Disponible"; // "Disponible", "Vendida"

        public PropertyType PropertyType { get; set; }
        public SaleType SaleType { get; set; }

        public virtual ICollection<PropertyImage> Images { get; set; }
        public virtual ICollection<PropertyImprovement> PropertyImprovements { get; set; }
        public virtual ICollection<FavoriteProperty> Favorites { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }

}
