using RealEstateApp.Core.Application.ViewModels.Client;

public interface IMessageService
{
    Task<List<MessageViewModel>> GetMessagesAsync(int propertyId, string clientId, string agentId);
    Task SendMessageAsync(string senderId, string receiverId, int propertyId, string content, bool isFromClient);


}
