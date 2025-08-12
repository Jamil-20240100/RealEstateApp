using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class SalesTypeRepository : ISalesTypeRepository
    {
        private readonly RealEstateContext _context;

        public SalesTypeRepository(RealEstateContext context)
        {
            _context = context;
        }

        public async Task<SalesType?> GetByIdAsync(int id)
        {
            return await _context.SalesTypes.FindAsync(id);
            // Si necesitas incluir colecciones relacionadas, usa:
            // return await _context.SalesTypes.Include(x => x.Properties).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<SalesType>> GetAllAsync()
        {
            return await _context.SalesTypes.ToListAsync();
        }

        public async Task<SalesType> AddAsync(SalesType entity)
        {
            await _context.SalesTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(SalesType entity)
        {
            _context.SalesTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SalesType entity)
        {
            _context.SalesTypes.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<SalesType> GetAllQuery()
        {
            return _context.SalesTypes.AsQueryable();
        }
    }
}
