using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class OfferRepository : GenericRepository<Offer>, IOfferRepository
    {
        private readonly RealEstateContext _context;

        public OfferRepository(RealEstateContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Offer?> GetByIdAsync(int id)
        {
            return await _context.Offers.FindAsync(id);
        }


        public async Task<List<Offer>> GetByPropertyAndClientAsync(int propertyId, string clientId)
        {
            return await _context.Offers
                .Where(o => o.PropertyId == propertyId && o.ClientId == clientId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<List<Offer>> GetAllPendingByPropertyAsync(int propertyId, int excludeOfferId = 0)
        {
            return await _context.Offers
                .Where(o => o.PropertyId == propertyId && o.Status == Core.Domain.Common.Enums.OfferStatus.Pendiente && o.Id != excludeOfferId)
                .ToListAsync();
        }
    }
}
