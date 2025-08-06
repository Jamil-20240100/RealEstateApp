using RealEstateApp.Core.Domain.Common.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Client
{
    public class OfferViewModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string ClientId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OfferStatus Status { get; set; }
    }

}
