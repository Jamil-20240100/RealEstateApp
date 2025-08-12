using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IFeatureRepository : IGenericRepository<Feature>
    {
        Task<Feature?> GetByIdAsync(int id);
    }
}
