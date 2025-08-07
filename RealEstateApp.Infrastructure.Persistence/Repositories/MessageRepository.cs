using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly RealEstateContext _context;

        public MessageRepository(RealEstateContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetAllAsync()
        {
            return await _context.Messages.ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByPropertyAndUsersAsync(int propertyId, string clientId, string agentId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId &&
                           ((m.SenderId == clientId && m.ReceiverId == agentId) ||
                            (m.SenderId == agentId && m.ReceiverId == clientId)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesByPropertyAndAgentAsync(int propertyId, string agentId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId &&
                           (m.SenderId == agentId || m.ReceiverId == agentId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesByPropertyAsync(int propertyId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
