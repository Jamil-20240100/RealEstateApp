using RealEstateApp.Core.Application.ViewModels.Property;

public interface IFavoriteService
{
    Task ToggleFavoriteAsync(string userId, int propertyId);
    Task<List<PropertyViewModel>> GetFavoritePropertiesByUserAsync(string userId);
    Task<bool> IsFavoriteAsync(string userId, int propertyId);
    
}
