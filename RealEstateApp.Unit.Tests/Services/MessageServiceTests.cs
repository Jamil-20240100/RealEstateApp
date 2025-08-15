using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;
using RealEstateApp.Core.Application.Mappings.DTOsAndViewModels;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;

namespace RealEstateApp.Unit.Tests.Services
{
    public class MessageServiceTests
    {
        private readonly DbContextOptions<RealEstateContext> _dbOptions;
        private readonly IMapper _mapper;

        public MessageServiceTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealEstateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_MessageService_{Guid.NewGuid()}")
                .Options;

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MessageMappingProfile>();
                cfg.AddProfile<MessageDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private MessageService CreateService(RealEstateContext context)
        {
            var repo = new MessageRepository(context);
            return new MessageService(repo, _mapper);
        }

        [Fact]
        public async Task SendMessageAsync_Should_Save_Message()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            await service.SendMessageAsync("client1", "agent1", 1, "Hola agente", true);

            var messages = await context.Messages.ToListAsync();
            messages.Should().ContainSingle();
            messages.First().Content.Should().Be("Hola agente");
            messages.First().IsFromClient.Should().BeTrue();
        }

        [Fact]
        public async Task GetMessagesAsync_Should_Return_Ordered_Messages()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            await context.Messages.AddRangeAsync(
                new Message { SenderId = "client1", ReceiverId = "agent1", PropertyId = 1, Content = "Primero", SentAt = DateTime.Now.AddMinutes(-5), IsFromClient = true },
                new Message { SenderId = "agent1", ReceiverId = "client1", PropertyId = 1, Content = "Segundo", SentAt = DateTime.Now, IsFromClient = false }
            );
            await context.SaveChangesAsync();

            var result = await service.GetMessagesAsync(1, "client1", "agent1");

            result.Should().HaveCount(2);
            result.First().Content.Should().Be("Primero");
            result.Last().Content.Should().Be("Segundo");
        }

        [Fact]
        public async Task GetClientsWhoMessagedAgentForPropertyAsync_Should_Return_ClientIds()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            await context.Messages.AddRangeAsync(
                new Message { SenderId = "client1", ReceiverId = "agent1", PropertyId = 1, Content = "Hola", IsFromClient = true },
                new Message { SenderId = "client2", ReceiverId = "agent1", PropertyId = 1, Content = "Buenas", IsFromClient = true },
                new Message { SenderId = "agent1", ReceiverId = "client1", PropertyId = 1, Content = "Respuesta", IsFromClient = false }
            );
            await context.SaveChangesAsync();

            var clientIds = await service.GetClientsWhoMessagedAgentForPropertyAsync("agent1", 1);

            clientIds.Should().Contain(new[] { "client1", "client2" });
        }

        [Fact]
        public async Task GetMessagesForPropertyAsync_Should_Return_Messages_Ordered()
        {
            using var context = new RealEstateContext(_dbOptions);
            var service = CreateService(context);

            await context.Messages.AddRangeAsync(
                new Message { SenderId = "client1", ReceiverId = "agent1", PropertyId = 2, Content = "Mensaje 1", SentAt = DateTime.Now.AddMinutes(-10), IsFromClient = true },
                new Message { SenderId = "agent1", ReceiverId = "client1", PropertyId = 2, Content = "Mensaje 2", SentAt = DateTime.Now, IsFromClient = false }
            );
            await context.SaveChangesAsync();

            var result = await service.GetMessagesForPropertyAsync(2);

            result.Should().HaveCount(2);
            result.First().Content.Should().Be("Mensaje 1");
            result.Last().Content.Should().Be("Mensaje 2");
        }
    }
}
