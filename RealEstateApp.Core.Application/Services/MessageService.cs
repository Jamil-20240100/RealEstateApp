using AutoMapper;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepo;
    private readonly IMapper _mapper;

    public MessageService(IMessageRepository messageRepo, IMapper mapper)
    {
        _messageRepo = messageRepo;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todos los mensajes entre el cliente y el agente de una propiedad
    /// </summary>
    public async Task<List<MessageViewModel>> GetMessagesAsync(int propertyId, string? clientId, string agentId)
    {
        List<Message> messages;

        if (string.IsNullOrEmpty(clientId))
        {
            // Trae todos los mensajes de esa propiedad donde el agente participa
            messages = await _messageRepo.GetMessagesByPropertyAndAgentAsync(propertyId, agentId);
        }
        else
        {
            // Trae solo los mensajes entre cliente y agente
            messages = await _messageRepo.GetMessagesByPropertyAndUsersAsync(propertyId, clientId, agentId);
        }

        messages = messages.OrderBy(m => m.SentAt).ToList();

        var dtoList = _mapper.Map<List<MessageDTO>>(messages);
        return _mapper.Map<List<MessageViewModel>>(dtoList);
    }
    /// <summary>
    /// Envía un mensaje de cliente a agente o viceversa
    /// </summary>
    public async Task SendMessageAsync(string senderId, string receiverId, int propertyId, string content, bool isFromClient)
    {
        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            PropertyId = propertyId,
            Content = content,
            IsFromClient = isFromClient,
            SentAt = DateTime.Now
        };

        await _messageRepo.AddAsync(message);
    }

    public async Task<List<string>> GetClientsWhoMessagedAgentForPropertyAsync(string agentId, int propertyId)
    {
        var messages = await _messageRepo.GetMessagesByPropertyAndAgentAsync(propertyId, agentId);

        var clientIds = messages
            .Where(m => m.SenderId != agentId) // Filtrar para solo clientes
            .Select(m => m.SenderId)
            .Distinct()
            .ToList();

        return clientIds;
    }

    public async Task<List<MessageViewModel>> GetMessagesForPropertyAsync(int propertyId)
    {
        var messages = await _messageRepo.GetMessagesByPropertyAsync(propertyId);

        var dtoList = messages
            .OrderBy(m => m.SentAt)
            .Select(m => _mapper.Map<MessageDTO>(m))
            .ToList();

        var vmList = _mapper.Map<List<MessageViewModel>>(dtoList);
        return vmList;
    }
}
