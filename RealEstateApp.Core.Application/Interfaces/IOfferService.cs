using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IOfferService
{
        Task<List<OfferViewModel>> GetOffersByClientAsync(int propertyId, string clientId);
        Task CreateOfferAsync(string clientId, int propertyId, decimal amount);
        Task UpdateOfferStatusAsync(int offerId, OfferStatus status);
        Task<int> GetPropertyIdByOfferAsync(int offerId);
        Task RejectOfferAsync(int offerId);
        Task AcceptOfferAsync(int offerId);
        Task<List<string>> GetClientsWithOffersForPropertyAsync(int propertyId);
        Task<List<OfferViewModel>> GetOffersForPropertyAsync(int propertyId);
}
