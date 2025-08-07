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
                property.State = PropertyState.Vendida;
                await _propertyRepo.UpdateAsync(property);
            }
        }
    }

    public async Task<List<string>> GetClientsWithOffersAsync(int propertyId)
    {
        var offers = await _offerRepo.GetAllAsync();

        var clientIds = offers
            .Where(o => o.PropertyId == propertyId)
            .Select(o => o.ClientId)
            .Distinct()
            .ToList();

        return clientIds;
    }

    public async Task<List<OfferViewModel>> GetOffersByClientAndPropertyAsync(string clientId, int propertyId)
    {
        var offers = await _offerRepo.GetAllAsync();

        var filtered = offers
            .Where(o => o.ClientId == clientId && o.PropertyId == propertyId)
            .OrderByDescending(o => o.Date)
            .ToList();

        return _mapper.Map<List<OfferViewModel>>(filtered);
    }

    public async Task RespondToOfferAsync(int offerId, bool isAccepted)
    {
        var offer = await _offerRepo.GetByIdAsync(offerId);
        if (offer == null)
            throw new Exception("Oferta no encontrada.");

        var property = await _propertyRepo.GetByIdAsync(offer.PropertyId);
        if (property == null)
            throw new Exception("Propiedad no encontrada.");

        if (isAccepted)
        {
            offer.Status = OfferStatus.Aceptada; // Asume que tienes enum OfferStatus
            // Rechazar todas las ofertas pendientes para esa propiedad
            var pendingOffers = await _offerRepo.GetAllAsync();
            var toReject = pendingOffers
                .Where(o => o.PropertyId == property.Id && o.Status == OfferStatus.Pendiente && o.Id != offerId);

            foreach (var o in toReject)
            {
                o.Status = OfferStatus.Rechazada;
                await _offerRepo.UpdateAsync(o);
            }

            // Cambiar estado propiedad a Vendida
            property.State = PropertyState.Vendida; // Asume enum PropertyState
            await _propertyRepo.UpdateAsync(property);
        }
        else
        {
            offer.Status = OfferStatus.Rechazada;
        }

        await _offerRepo.UpdateAsync(offer);
    }

    public async Task<List<string>> GetClientsWithOffersForPropertyAsync(int propertyId)
    {
        var offers = await _offerRepo.GetAllAsync(); // Este método deberías implementarlo o traer ofertas por propiedad

        var clientIds = offers
            .Where(o => o.PropertyId == propertyId)
            .Select(o => o.ClientId)
            .Distinct()
            .ToList();

        return clientIds;
    }


    public async Task AcceptOfferAsync(int offerId)
    {
        var offer = await _offerRepo.GetByIdAsync(offerId);
        if (offer == null)
            throw new Exception("Oferta no encontrada.");

        offer.Status = OfferStatus.Aceptada;
        await _offerRepo.UpdateAsync(offer);

        var otherOffers = await _offerRepo.GetAllPendingByPropertyAsync(offer.PropertyId, offer.Id);
        foreach (var o in otherOffers)
        {
            o.Status = OfferStatus.Rechazada;
            await _offerRepo.UpdateAsync(o);
        }

        var property = await _propertyRepo.GetByIdAsync(offer.PropertyId);
        if (property != null)
        {
            property.State = PropertyState.Vendida;
            await _propertyRepo.UpdateAsync(property);
        }
    }

    public async Task RejectOfferAsync(int offerId)
    {
        var offer = await _offerRepo.GetByIdAsync(offerId);
        if (offer == null)
            throw new Exception("Oferta no encontrada.");

        offer.Status = OfferStatus.Rechazada;
        await _offerRepo.UpdateAsync(offer);
    }

    public async Task<int> GetPropertyIdByOfferAsync(int offerId)
    {
        var offer = await _offerRepo.GetByIdAsync(offerId);
        if (offer == null)
            throw new Exception("Oferta no encontrada.");

        return offer.PropertyId;
    }

    public async Task<List<OfferViewModel>> GetOffersForPropertyAsync(int propertyId)
    {
        var offers = await _offerRepo.GetOffersByPropertyAsync(propertyId);

        var orderedOffers = offers
            .OrderByDescending(o => o.Date)
            .ToList();

        return _mapper.Map<List<OfferViewModel>>(orderedOffers);
    }
}
