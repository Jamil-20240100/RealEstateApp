using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
    {
        public PropertyRepository(RealEstateContext context) : base(context)
        {
        }

        public async Task<Property?> GetByPropertyCodeAsync(string propertyCode)
        {
            return await _context.Properties
                            .FirstOrDefaultAsync(p => p.Code == propertyCode);
        }
    }
}
