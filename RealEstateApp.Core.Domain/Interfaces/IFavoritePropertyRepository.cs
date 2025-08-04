using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IFavoritePropertyRepository : IGenericRepository<FavoriteProperty>
{
    Task<FavoriteProperty> GetByUserAndPropertyAsync(string userId, int propertyId);
    Task<List<FavoriteProperty>> GetAllByUserIdAsync(string userId);
}
