using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class PropertyService : GenericService<Property, PropertyDTO>, IPropertyService
    {
        private readonly IPropertyRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPropertyTypeRepository _propertyTypeRepository;
        private readonly ISalesTypeRepository _salesTypeRepository;
        private readonly IFeatureRepository _featureRepository;

        public PropertyService(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISalesTypeRepository salesTypeRepository,
            IFeatureRepository featureRepository,
            IMapper mapper) : base(propertyRepository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _salesTypeRepository = salesTypeRepository;
            _featureRepository = featureRepository;
            _repository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<List<PropertyDTO>> GetAllWithInclude()
        {
            var properties = await _repository.GetAllWithInclude(["Images", "Features", "SalesType", "PropertyType"]);
            var mappedProperties = _mapper.Map<List<PropertyDTO>>(properties);
            return mappedProperties;
        }

        public async Task<PropertyDTO?> GetByIdWithInclude(int id)
        {
            var entity = await _repository.GetByIdWithInclude(id,["Images", "Features", "PropertyType", "SalesType"]);
            return entity == null ? null : _mapper.Map<PropertyDTO>(entity);
        }

        public async Task<PropertyDTO?> AddAsync(PropertyDTO dto)
        {
            var entity = _mapper.Map<Property>(dto);

            entity.Features = new List<Feature>();
            if (dto.Features != null)
            {
                foreach (var featureDto in dto.Features)
                {
                    var feature = await _featureRepository.GetById(featureDto.Id);
                    if (feature != null)
                    {
                        entity.Features.Add(feature);
                    }
                }
            }

            await _repository.AddAsync(entity);

            return _mapper.Map<PropertyDTO>(entity);
        }

        public override async Task<PropertyDTO?> UpdateAsync(PropertyDTO dto, int id)
        {
            var entity = await _repository.GetByIdWithInclude(id, new List<string> { "Features", "Images" });
            if (entity == null) return null;

            entity.Price = dto.Price;
            entity.Description = dto.Description;
            entity.SizeInMeters = dto.SizeInMeters;
            entity.NumberOfRooms = dto.NumberOfRooms;
            entity.NumberOfBathrooms = dto.NumberOfBathrooms;
            entity.PropertyTypeId = dto.PropertyType.Id;
            entity.SalesTypeId = dto.SalesType.Id;

            var newFeatureIds = dto.Features?.Select(f => f.Id).ToList() ?? new List<int>();
            var featuresToRemove = entity.Features.Where(f => !newFeatureIds.Contains(f.Id)).ToList();
            foreach (var feature in featuresToRemove)
                entity.Features.Remove(feature);
            foreach (var featureId in newFeatureIds)
            {
                if (!entity.Features.Any(f => f.Id == featureId))
                {
                    var feature = await _featureRepository.GetById(featureId);
                    if (feature != null)
                        entity.Features.Add(feature);
                }
            }

            var newImageUrls = dto.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>();
            var imagesToRemove = entity.Images.Where(img => !newImageUrls.Contains(img.ImageUrl)).ToList();
            foreach (var img in imagesToRemove)
                entity.Images.Remove(img);

            foreach (var newImageDto in dto.Images)
            {
                if (!entity.Images.Any(img => img.ImageUrl == newImageDto.ImageUrl))
                    entity.Images.Add(new PropertyImage { ImageUrl = newImageDto.ImageUrl });
            }

            await _repository.UpdateAsync(entity);

            return _mapper.Map<PropertyDTO>(entity);
        }

        public async Task<string> GenerateUniquePropertyCodeAsync()
        {
            const int maxAttempts = 9999;
            var random = new Random();

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                string candidate = random.Next(000000, 999999).ToString();

                var existingProperty = await _repository.GetByPropertyCodeAsync(candidate);
                if (existingProperty == null)
                    return candidate;
            }

            throw new Exception("No se pudo generar un codigo de propiedad único luego de varios intentos.");
        }
    }
}
