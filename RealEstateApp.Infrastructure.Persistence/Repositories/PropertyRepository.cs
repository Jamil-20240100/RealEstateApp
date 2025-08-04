using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    private readonly RealEstateContext _context;

    public PropertyRepository(RealEstateContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Property>> GetAvailableWithFiltersAsync(
        int? propertyTypeId, decimal? minPrice, decimal? maxPrice, int? bathrooms, int? bedrooms)
    {
        var query = _context.Properties
            .Where(p => p.State == "Disponible")
            .Include(p => p.Images)
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .AsQueryable();

        if (propertyTypeId.HasValue)
            query = query.Where(p => p.PropertyTypeId == propertyTypeId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (bathrooms.HasValue)
            query = query.Where(p => p.Bathrooms == bathrooms.Value);

        if (bedrooms.HasValue)
            query = query.Where(p => p.Bedrooms == bedrooms.Value);

        return await query.OrderByDescending(p => p.Id).ToListAsync();
    }

    public async Task<Property?> GetByIdAsync(int id)
    {
        return await _context.Properties.FindAsync(id);
    }

    public async Task<Property?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.PropertyImprovements)
                .ThenInclude(pi => pi.Improvement)
            .Include(p => p.Offers)
            .Include(p => p.Messages)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

}


