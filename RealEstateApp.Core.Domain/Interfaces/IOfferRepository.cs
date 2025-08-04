using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IOfferRepository : IGenericRepository<Offer>
    {
        Task<Offer?> GetByIdAsync(int id);
        Task<List<Offer>> GetByPropertyAndClientAsync(int propertyId, string clientId);
        Task<List<Offer>> GetAllPendingByPropertyAsync(int propertyId, int excludeOfferId = 0);
    }
}
