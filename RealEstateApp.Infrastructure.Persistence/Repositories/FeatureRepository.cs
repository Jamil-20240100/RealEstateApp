using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class FeatureRepository : GenericRepository<Feature>, IFeatureRepository
    {
        private readonly RealEstateContext _dbContext;

        public FeatureRepository(RealEstateContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Feature?> GetByIdAsync(int id)
        {
            return await _dbContext.Set<Feature>()
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
