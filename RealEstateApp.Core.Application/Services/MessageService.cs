using AutoMapper;
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
    public async Task<List<MessageViewModel>> GetMessagesAsync(int propertyId, string clientId, string agentId)
    {
        var messages = await _messageRepo.GetMessagesByPropertyAndUsersAsync(propertyId, clientId, agentId);

        return messages
            .OrderBy(m => m.SentAt)
            .Select(m => _mapper.Map<MessageViewModel>(m))
            .ToList();
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
}
