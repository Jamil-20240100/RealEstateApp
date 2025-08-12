using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class FavoritePropertyRepository : GenericRepository<FavoriteProperty>, IFavoritePropertyRepository
{
    private readonly RealEstateContext _context;

    public FavoritePropertyRepository(RealEstateContext context) : base(context)
    {
        _context = context;
    }

    public async Task<FavoriteProperty> GetByUserAndPropertyAsync(string userId, int propertyId)
    {
        return await _context.FavoriteProperties
            .FirstOrDefaultAsync(fp => fp.ClientId == userId && fp.PropertyId == propertyId);
    }

    public async Task<List<FavoriteProperty>> GetAllByUserIdAsync(string userId)
    {
        return await _context.FavoriteProperties
            .Include(f => f.Property)
                .ThenInclude(p => p.PropertyType)
            .Include(f => f.Property)
                .ThenInclude(p => p.SalesType)
            .Include(f => f.Property)
                .ThenInclude(p => p.Images)
            .Where(f => f.ClientId == userId)
            .ToListAsync();
    }
}
