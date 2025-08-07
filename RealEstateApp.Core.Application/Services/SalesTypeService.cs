using AutoMapper;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class SalesTypeService : GenericService<SalesType, SalesTypeDTO>, ISalesTypeService
    {
        public SalesTypeService(IGenericRepository<SalesType> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
