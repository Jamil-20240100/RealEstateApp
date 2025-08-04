using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepo;
    private readonly IFavoritePropertyRepository _favoriteRepo;
    private readonly IMapper _mapper;

    public PropertyService(IPropertyRepository propertyRepo, IFavoritePropertyRepository favoriteRepo, IMapper mapper)
    {
        _propertyRepo = propertyRepo;
        _favoriteRepo = favoriteRepo;
        _mapper = mapper;
    }

    public async Task<List<PropertyViewModel>> GetFilteredAvailableAsync(PropertyFilterViewModel filters, string? userId)
    {
        var properties = await _propertyRepo.GetAvailableWithFiltersAsync(
            filters.PropertyTypeId, filters.MinPrice, filters.MaxPrice, filters.Bathrooms, filters.Bedrooms);

        var result = _mapper.Map<List<PropertyViewModel>>(properties);

        if (!string.IsNullOrEmpty(userId))
        {
            var favorites = await _favoriteRepo.GetAllByUserIdAsync(userId);
            var favoriteIds = favorites.Select(f => f.PropertyId).ToHashSet();

            foreach (var item in result)
                item.IsFavorite = favoriteIds.Contains(item.Id);
        }

        return result;
    }

    public async Task<PropertyViewModel?> GetPropertyDetailsAsync(int id, string? userId)
    {
        var property = await _propertyRepo.GetByIdWithDetailsAsync(id);
        if (property == null) return null;

        var vm = _mapper.Map<PropertyViewModel>(property);

        if (!string.IsNullOrEmpty(userId))
        {
            vm.IsFavorite = await _favoriteRepo.GetByUserAndPropertyAsync(userId, id) != null;
        }

        return vm;
    }
}

