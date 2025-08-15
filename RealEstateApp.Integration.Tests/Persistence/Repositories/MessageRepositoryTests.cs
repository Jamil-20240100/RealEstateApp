using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Integration.Tests.Persistence.Repositories
{
    public class MessageRepositoryTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;

        public MessageRepositoryTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_Messages_{Guid.NewGuid()}")
                .Options;
        }

        private Message CreateMessage(int id, int propertyId, string senderId, string receiverId, string content, bool isFromClient = true)
        {
            return new Message
            {
                Id = id,
                PropertyId = propertyId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.Now.AddMinutes(id),
                IsFromClient = isFromClient
            };
        }

        [Fact]
        public async Task AddAsync_Should_Add_Message()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new MessageRepository(context);

            var message = CreateMessage(0, 1, "client1", "agent1", "Hello!");
            var result = await repo.AddAsync(message);

            result.Should().NotBeNull();
            result!.Id.Should().BeGreaterThan(0);

            var all = await context.Messages.ToListAsync();
            all.Should().ContainSingle();
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Message_When_Exists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var message = CreateMessage(0, 1, "client1", "agent1", "Hello again");
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetById(message.Id);

            result.Should().NotBeNull();
            result!.Content.Should().Be("Hello again");
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_When_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new MessageRepository(context);

            var result = await repo.GetById(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Messages()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Messages.AddRangeAsync(
                CreateMessage(0, 1, "client1", "agent1", "Hi"),
                CreateMessage(0, 2, "client2", "agent2", "Hey")
            );
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetAllAsync();

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetMessagesByPropertyAndUsersAsync_Should_Return_Correct_Conversation()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Messages.AddRangeAsync(
                CreateMessage(0, 1, "client1", "agent1", "Msg1"),
                CreateMessage(0, 1, "agent1", "client1", "Msg2"),
                CreateMessage(0, 1, "client2", "agent1", "Msg3"),
                CreateMessage(0, 2, "client1", "agent1", "Msg4")
            );
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetMessagesByPropertyAndUsersAsync(1, "client1", "agent1");

            result.Should().HaveCount(2);
            result.First().Content.Should().Be("Msg1");
        }

        [Fact]
        public async Task GetMessagesByPropertyAndAgentAsync_Should_Return_Messages_Involving_Agent()
        {
            using var context = new RealEstateContext(_dbOptions);
            await context.Messages.AddRangeAsync(
                CreateMessage(0, 1, "client1", "agent1", "Hi agent"),
                CreateMessage(0, 1, "agent1", "client2", "Hello client2"),
                CreateMessage(0, 1, "client3", "client4", "No agent here")
            );
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetMessagesByPropertyAndAgentAsync(1, "agent1");

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetMessagesByPropertyAsync_Should_Return_All_Messages_Ordered_By_Date()
        {
            using var context = new RealEstateContext(_dbOptions);
            var msg1 = CreateMessage(1, 5, "client1", "agent1", "Old message");
            msg1.SentAt = DateTime.Now.AddMinutes(-10);

            var msg2 = CreateMessage(2, 5, "agent1", "client1", "Recent message");
            msg2.SentAt = DateTime.Now;

            await context.Messages.AddRangeAsync(msg1, msg2);
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            var result = await repo.GetMessagesByPropertyAsync(5);

            result.Should().HaveCount(2);
            result.First().Content.Should().Be("Old message"); // ordered asc by SentAt
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Message()
        {
            using var context = new RealEstateContext(_dbOptions);
            var message = CreateMessage(0, 1, "client1", "agent1", "Delete me");
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            var repo = new MessageRepository(context);
            await repo.DeleteAsync(message.Id);

            var result = await repo.GetById(message.Id);
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_Should_Not_Throw_When_Id_NotExists()
        {
            using var context = new RealEstateContext(_dbOptions);
            var repo = new MessageRepository(context);

            Func<Task> act = async () => await repo.DeleteAsync(999);
            await act.Should().NotThrowAsync();
        }
    }
}
