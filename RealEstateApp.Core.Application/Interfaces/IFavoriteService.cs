using RealEstateApp.Core.Application.ViewModels.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IFavoriteService
{
    Task ToggleFavoriteAsync(string userId, int propertyId);
    Task<List<PropertyViewModel>> GetFavoritePropertiesByUserAsync(string userId);
    Task<bool> IsFavoriteAsync(string userId, int propertyId);
    
}
