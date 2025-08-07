using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IPropertyService : IGenericService<PropertyDTO>
    {
        public Task<List<PropertyDTO>> GetAllWithInclude();
        public Task<PropertyDTO?> GetByIdWithInclude(int id);
        Task<PropertyDTO?> AddAsync(PropertyDTO dto);
        Task<PropertyDTO?> UpdateAsync(PropertyDTO dto, int id);

        public Task<string> GenerateUniquePropertyCodeAsync();

        //Para cuando agreguemos el filtro
        Task<List<PropertyViewModel>> GetFilteredAvailableAsync(PropertyFilterViewModel? filters, string? userId);
        Task<PropertyDetailsViewModel?> GetPropertyDetailsAsync(int id, string? userId);
        Task<string?> GetAgentIdByPropertyIdAsync(int propertyId);

    }
}
