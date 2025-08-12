using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class PropertyTypeRepository : GenericRepository<PropertyType>, IPropertyTypeRepository
    {
        private readonly RealEstateContext _context;

        public PropertyTypeRepository(RealEstateContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PropertyType?> GetByIdAsync(int id)
        {
            // Si necesitas las propiedades relacionadas (para contar o para validaciones), 
            // descomenta el Include. Si no, FindAsync es más barato.
            return await _context.PropertyTypes
                                 //.Include(pt => pt.Properties) // descomenta si necesitas las propiedades
                                 .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public IQueryable<PropertyType> GetAllQuery()
        {
            return _context.PropertyTypes.AsNoTracking();
        }
    }
}