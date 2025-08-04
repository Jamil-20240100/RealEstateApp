using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPropertyRepository : IGenericRepository<Property>
{
    Task<List<Property>> GetAvailableWithFiltersAsync(
        int? propertyTypeId, decimal? minPrice, decimal? maxPrice, int? bathrooms, int? bedrooms);

    Task<Property?> GetByIdAsync(int id);

    /// <summary>
    /// Trae la propiedad con imágenes, tipo, venta, mejoras, ofertas y mensajes
    /// </summary>
    Task<Property?> GetByIdWithDetailsAsync(int id);
}

