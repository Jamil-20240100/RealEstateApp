using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class FeatureService : GenericService<Feature, FeatureDTO>, IFeatureService
    {
        public FeatureService(IGenericRepository<Feature> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
