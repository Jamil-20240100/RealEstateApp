using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface ISalesTypeRepository
    {
        Task<SalesType?> GetByIdAsync(int id);
        Task<List<SalesType>> GetAllAsync();
        Task<SalesType> AddAsync(SalesType entity);
        Task UpdateAsync(SalesType entity);
        Task DeleteAsync(SalesType entity);

        // Opcionales pero útiles para queries en features/handlers
        IQueryable<SalesType> GetAllQuery();
    }
}
