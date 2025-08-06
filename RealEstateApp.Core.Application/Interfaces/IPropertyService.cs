using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<PropertyDTO>
    {
        public Task<List<PropertyDTO>> GetAllWithInclude();
        public Task<PropertyDTO?> GetByIdWithInclude(int id);
        Task<PropertyDTO?> AddAsync(PropertyDTO dto);
        Task<PropertyDTO?> UpdateAsync(PropertyDTO dto, int id);
    }
}
