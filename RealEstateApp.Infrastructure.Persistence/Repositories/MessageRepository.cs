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

        public async Task<List<Message>> GetMessagesByPropertyAndUsersAsync(int propertyId, string clientId, string agentId)
        {
            return await _context.Messages
                .Where(m => m.PropertyId == propertyId &&
                           ((m.SenderId == clientId && m.ReceiverId == agentId) ||
                            (m.SenderId == agentId && m.ReceiverId == clientId)))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}
