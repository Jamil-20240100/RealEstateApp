using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyTypeRepository : IGenericRepository<PropertyType>
    {
        // Devuelve la entidad por id (incluyendo, si quieres, relaciones)
        Task<PropertyType?> GetByIdAsync(int id);

        // Permite consultas LINQ externas cuando lo necesites (por ejemplo en handlers MediatR)
        IQueryable<PropertyType> GetAllQuery();
    }
}
