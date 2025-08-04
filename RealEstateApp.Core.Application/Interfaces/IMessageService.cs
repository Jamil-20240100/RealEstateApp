using RealEstateApp.Core.Application.ViewModels.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IMessageService
{
    Task<List<MessageViewModel>> GetMessagesAsync(int propertyId, string clientId, string agentId);
    Task SendMessageAsync(string senderId, string receiverId, int propertyId, string content, bool isFromClient);


}
