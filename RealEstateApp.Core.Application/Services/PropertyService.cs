using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.ViewModels.Property;
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
        private readonly IFavoritePropertyRepository _favoriteRepository;

        public PropertyService(
            IPropertyRepository propertyRepository,
            IPropertyTypeRepository propertyTypeRepository,
            ISalesTypeRepository salesTypeRepository,
            IFeatureRepository featureRepository,
            IFavoritePropertyRepository favoriteRepository,
            IMapper mapper) : base(propertyRepository, mapper)
        {
            _propertyTypeRepository = propertyTypeRepository;
            _salesTypeRepository = salesTypeRepository;
            _featureRepository = featureRepository;
            _repository = propertyRepository;
            _mapper = mapper;
            _favoriteRepository = favoriteRepository;
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

        //Este es el fr pero hasta que hagamos los filtros usaremos el otro de base

        //public async Task<List<PropertyViewModel>> GetFilteredAvailableAsync(PropertyFilterViewModel filters, string? userId)
        //{
        //    // 1) Filtros base soportados por tu repo
        //    var baseEntities = await _repository.GetAvailableWithFiltersAsync(
        //        filters.PropertyType?.Id,            // int? PropertyTypeId
        //        filters.MinPrice,                   // decimal?
        //        filters.MaxPrice,                   // decimal?
        //        filters.MinNumberOfBathrooms,       // int?  (si tu repo espera "Bathrooms exactos", ajusta aquí)
        //        filters.MinNumberOfRooms            // int?  (ídem)
        //    );

        //    // 2) Refinar en memoria lo que falte (SalesType, CodeContains, Size, Features, OnlyFavorites)
        //    var query = baseEntities.AsQueryable();

        //    if (filters.SalesType?.Id is int saleTypeId)
        //        query = query.Where(p => p.SalesTypeId == saleTypeId);

        //    if (!string.IsNullOrWhiteSpace(filters.CodeContains))
        //        query = query.Where(p => p.Code != null && p.Code.Contains(filters.CodeContains));

        //    if (filters.MinSizeInMeters.HasValue)
        //        query = query.Where(p => p.SizeInMeters >= filters.MinSizeInMeters.Value);

        //    if (filters.MaxSizeInMeters.HasValue)
        //        query = query.Where(p => p.SizeInMeters <= filters.MaxSizeInMeters.Value);

        //    if (filters.MinNumberOfRooms.HasValue)
        //        query = query.Where(p => p.NumberOfRooms >= filters.MinNumberOfRooms.Value);

        //    if (filters.MinNumberOfBathrooms.HasValue)
        //        query = query.Where(p => p.NumberOfBathrooms >= filters.MinNumberOfBathrooms.Value);

        //    if (filters.FeatureIds != null && filters.FeatureIds.Count > 0)
        //        query = query.Where(p => p.Features != null && filters.FeatureIds.All(fid => p.Features.Any(f => f.Id == fid)));

        //    // 3) Solo favoritos (si lo piden y hay userId)
        //    if (filters.OnlyFavorites && !string.IsNullOrEmpty(userId))
        //    {
        //        var favIds = (await _favoriteRepository.GetAllByUserIdAsync(userId))
        //                     .Select(f => f.PropertyId)
        //                     .ToHashSet();
        //        query = query.Where(p => favIds.Contains(p.Id));
        //    }

        //    var entities = query.ToList();

        //    // 4) Mapear a tu ViewModel
        //    var list = _mapper.Map<List<PropertyViewModel>>(entities);

        //    // (Opcional) Si necesitas los favoritos en la vista y tu VM no tiene IsFavorite,
        //    // desde el controlador puedes pedir el HashSet y pasarlo por ViewBag/ViewData.

        //    return list;
        //}


        public async Task<List<PropertyViewModel>> GetFilteredAvailableAsync(PropertyFilterViewModel? filters, string? userId)
        {
            // Filtro por defecto (sin restricciones)
            filters ??= new PropertyFilterViewModel();

            var baseEntities = await _repository.GetAvailableWithFiltersAsync(
                filters.PropertyType?.Id,     // será null al inicio
                filters.MinPrice,             // null
                filters.MaxPrice,             // null
                filters.MinNumberOfBathrooms, // null
                filters.MinNumberOfRooms      // null
            );

            var query = baseEntities.AsQueryable();

            // (De momento no refinas nada: sin code, sales type, size, features…)
            // Más adelante solo agregas condiciones aquí cuando actives el filtro.

            // Si quisieras soportar "Solo favoritos" cuando añadas login:
            if (filters.OnlyFavorites && !string.IsNullOrEmpty(userId))
            {
                var favIds = (await _favoriteRepository.GetAllByUserIdAsync(userId))
                             .Select(f => f.PropertyId)
                             .ToHashSet();
                query = query.Where(p => favIds.Contains(p.Id));
            }

            var entities = query.ToList();
            var dtos = _mapper.Map<List<PropertyDTO>>(entities);
            var vms = _mapper.Map<List<PropertyViewModel>>(dtos);
            return vms;
        }



        // ===============================
        // DETALLE: mapea Property -> DTO -> ViewModel
        // ===============================
        public async Task<PropertyDetailsViewModel?> GetPropertyDetailsAsync(int id, string? userId)
        {
            var entity = await _repository.GetByIdWithDetailsAsync(id);
            if (entity == null) return null;

            // 1) Entidad -> DTO (usa PropertyMappingProfile)
            var dto = _mapper.Map<PropertyDTO>(entity);

            // 2) DTO -> ViewModel (usa PropertyDTOMappingProfile)
            var vm = _mapper.Map<PropertyDetailsViewModel>(dto);

            // (Opcional) Si quieres conocer si es favorito:
            if (!string.IsNullOrEmpty(userId))
            {
                var fav = await _favoriteRepository.GetByUserAndPropertyAsync(userId, id);
                // Tu VM no tiene IsFavorite; si lo necesitas, agrégalo al VM o pásalo por ViewBag desde el controller.
            }

            return vm;
        }


        public async Task<string?> GetAgentIdByPropertyIdAsync(int propertyId)
        {
            var property = await _repository.GetByIdAsync(propertyId);

            if (property == null)
                return null;

          
            return property.AgentId;

        }


    }
}
