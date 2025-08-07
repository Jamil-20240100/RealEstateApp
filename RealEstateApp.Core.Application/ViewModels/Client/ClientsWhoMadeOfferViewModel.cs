using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateApp.Core.Application.ViewModels.Client
{
    public class ClientsWhoMadeOfferViewModel
    {
        public required string Id { get; set; }                  // ID del cliente
        public required string FullName { get; set; }            // Nombre completo
        public required string ProfileImageUrl { get; set; }     // URL de imagen
        public required decimal OfferAmount { get; set; }        // Monto de la oferta
    }
}
