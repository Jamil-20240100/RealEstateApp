using AutoMapper;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyTypeService : GenericService<PropertyType, PropertyTypeDTO>, IPropertyTypeService
    {
        public PropertyTypeService(IGenericRepository<PropertyType> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
