using AutoMapper;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepo;
    private readonly IPropertyRepository _propertyRepo;
    private readonly IMapper _mapper;

    public OfferService(IOfferRepository offerRepo, IPropertyRepository propertyRepo, IMapper mapper)
    {
        _offerRepo = offerRepo;
        _propertyRepo = propertyRepo;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todas las ofertas del cliente para una propiedad
    /// </summary>
    public async Task<List<OfferViewModel>> GetOffersByClientAsync(int propertyId, string clientId)
    {
        var offers = await _offerRepo.GetByPropertyAndClientAsync(propertyId, clientId);
        return offers
            .OrderByDescending(o => o.Date)
            .Select(o => _mapper.Map<OfferViewModel>(o))
            .ToList();
    }

    /// <summary>
    /// Crea una nueva oferta en estado pendiente
    /// </summary>
    public async Task CreateOfferAsync(string clientId, int propertyId, decimal amount)
    {
        // Validación: no crear si existe oferta pendiente o aceptada del cliente
        var existingOffers = await _offerRepo.GetByPropertyAndClientAsync(propertyId, clientId);
        if (existingOffers.Any(o => o.Status == OfferStatus.Pendiente || o.Status == OfferStatus.Aceptada))
            return;

        var offer = new Offer
        {
            ClientId = clientId,
            PropertyId = propertyId,
            Amount = amount,
            Date = DateTime.Now,
            Status = OfferStatus.Pendiente
        };

        await _offerRepo.AddAsync(offer);
    }

    /// <summary>
    /// Cambia el estado de una oferta (uso de agente)
    /// </summary>
    public async Task UpdateOfferStatusAsync(int offerId, OfferStatus status)
    {
        var offer = await _offerRepo.GetByIdAsync(offerId);
        if (offer == null) return;

        offer.Status = status;
        await _offerRepo.UpdateAsync(offer);

        // Si se acepta la oferta, se deben rechazar las demás ofertas pendientes
        if (status == OfferStatus.Aceptada)
        {
            var otherOffers = await _offerRepo.GetAllPendingByPropertyAsync(offer.PropertyId, offer.Id);
            foreach (var o in otherOffers)
            {
                o.Status = OfferStatus.Rechazada;
                await _offerRepo.UpdateAsync(o);
            }

            // Cambiar propiedad a "Vendida"
            var property = await _propertyRepo.GetByIdAsync(offer.PropertyId);
            if (property != null)
            {
                property.State = "Vendida";
                await _propertyRepo.UpdateAsync(property);
            }
        }
    }
}
