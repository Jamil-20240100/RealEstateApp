using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {

        Task<Property?> GetByPropertyCodeAsync(string propertyCode);

        Task<List<Property>> GetAvailableWithFiltersAsync(
            int? propertyTypeId, decimal? minPrice, decimal? maxPrice, int? bathrooms, int? bedrooms);

        Task<Property?> GetByIdAsync(int id);

        /// <summary>
        /// Trae la propiedad con imágenes, tipo, venta, mejoras, ofertas y mensajes
        /// </summary>
        Task<Property?> GetByIdWithDetailsAsync(int id);

    }
}
