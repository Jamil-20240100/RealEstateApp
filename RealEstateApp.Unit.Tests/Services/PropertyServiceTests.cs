using Moq;
using AutoMapper;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Application.Mappings.DTOsAndViewModels;
using RealEstateApp.Core.Application.Mappings.EntitiesAndDTOs;
using RealEstateApp.Core.Application.DTOs.Message;
using RealEstateApp.Core.Application.DTOs.Offer;
using RealEstateApp.Core.Application.DTOs.Client;
using RealEstateApp.Core.Application.ViewModels.Client;
using RealEstateApp.Core.Application.ViewModels.Feature;
using RealEstateApp.Core.Application.ViewModels.PropertyType;
using RealEstateApp.Core.Application.ViewModels.SalesType;

namespace RealEstateApp.Unit.Tests.Services
{
    public class PropertyServiceTests
    {
        private readonly Mock<IPropertyRepository> _propertyRepositoryMock;
        private readonly Mock<IPropertyTypeRepository> _propertyTypeRepositoryMock;
        private readonly Mock<ISalesTypeRepository> _salesTypeRepositoryMock;
        private readonly Mock<IFeatureRepository> _featureRepositoryMock;
        private readonly Mock<IFavoritePropertyRepository> _favoriteRepositoryMock;
        private readonly IMapper _mapper;
        private readonly PropertyService _service;

        public PropertyServiceTests()
        {
            _propertyRepositoryMock = new Mock<IPropertyRepository>();
            _propertyTypeRepositoryMock = new Mock<IPropertyTypeRepository>();
            _salesTypeRepositoryMock = new Mock<ISalesTypeRepository>();
            _featureRepositoryMock = new Mock<IFeatureRepository>();
            _favoriteRepositoryMock = new Mock<IFavoritePropertyRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<FeatureMappingProfile>();
                cfg.AddProfile<PropertyTypeMappingProfile>();
                cfg.AddProfile<SalesTypeMappingProfile>();
                cfg.AddProfile<PropertyMappingProfile>();

                cfg.AddProfile<FeatureDTOMappingProfile>();
                cfg.AddProfile<PropertyTypeDTOMappingProfile>();
                cfg.AddProfile<SalesTypeDTOMappingProfile>();
                cfg.AddProfile<PropertyDTOMappingProfile>();
            });
            _mapper = config.CreateMapper();

            _service = new PropertyService(
                _propertyRepositoryMock.Object,
                _propertyTypeRepositoryMock.Object,
                _salesTypeRepositoryMock.Object,
                _featureRepositoryMock.Object,
                _favoriteRepositoryMock.Object,
                _mapper
            );
        }

        private PropertyDTO CreateSamplePropertyDTO()
        {
            return new PropertyDTO
            {
                Id = 1,
                AgentId = "agent-001",
                AgentName = "Juan Pérez",
                Price = 200000,
                Description = "Hermosa casa familiar en zona tranquila",
                SizeInMeters = 250,
                NumberOfRooms = 4,
                NumberOfBathrooms = 3,
                PropertyType = new PropertyTypeDTO { Id = 1, Name = "Casa", Description = "Casa familiar", NumberOfProperties = 5 },
                SalesType = new SalesTypeDTO { Id = 1, Name = "Venta", Description = "Venta directa", NumberOfProperties = 10 },
                Features = new List<FeatureDTO>
        {
            new FeatureDTO { Id = 1, Name = "Piscina", Description = "Piscina privada", NumberOfProperties = 2 },
            new FeatureDTO { Id = 2, Name = "Jardín", Description = "Jardín amplio", NumberOfProperties = 3 }
        },
                Images = new List<PropertyImageDTO>(),  // <-- inicializar para no mapear null
                Offers = new List<OfferDTO>(),          // <-- inicializar
                Messages = new List<MessageDTO>(),      // <-- inicializar
                ClientsWithOffers = new List<ClientDTO>(), // <-- inicializar
                Code = "PROP-001",
                State = PropertyState.Disponible,
                IsFavorite = false,
                IsSold = false
            };
        }

        private Property CreateSamplePropertyEntity()
        {
            return new Property
            {
                Id = 1,
                AgentId = "agent-001",
                AgentName = "Juan Pérez",
                Price = 200000,
                Description = "Hermosa casa familiar en zona tranquila",
                SizeInMeters = 250,
                NumberOfRooms = 4,
                NumberOfBathrooms = 3,
                PropertyTypeId = 1,
                PropertyType = new PropertyType { Id = 1, Name = "Casa", Description = "Casa familiar" },
                SalesTypeId = 1,
                SalesType = new SalesType { Id = 1, Name = "Venta", Description = "Venta directa" },
                Features = new List<Feature>
                {
                    new Feature { Id = 1, Name = "Piscina", Description = "Piscina privada" },
                    new Feature { Id = 2, Name = "Jardín", Description = "Jardín amplio" }
                },
                Code = "PROP-001",
                State = PropertyState.Disponible
            };
        }

        [Fact]
        public async Task GetByIdWithInclude_ShouldReturnMappedProperty()
        {
            var entity = CreateSamplePropertyEntity();
            _propertyRepositoryMock.Setup(r => r.GetByIdWithInclude(1, It.IsAny<List<string>>()))
                .ReturnsAsync(entity);

            var result = await _service.GetByIdWithInclude(1);

            Assert.NotNull(result);
            Assert.Equal(entity.Id, result!.Id);
            Assert.Equal(entity.AgentName, result.AgentName);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAndReturnProperty()
        {
            var dto = CreateSamplePropertyDTO();
            var entity = CreateSamplePropertyEntity();

            _featureRepositoryMock.Setup(f => f.GetById(1)).ReturnsAsync(entity.Features.First(f => f.Id == 1));
            _featureRepositoryMock.Setup(f => f.GetById(2)).ReturnsAsync(entity.Features.First(f => f.Id == 2));
            _propertyRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Property>())).ReturnsAsync((Property p) => p);

            var result = await _service.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.AgentName, result!.AgentName);
            Assert.Equal(dto.Features.Count, result.Features.Count);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePropertyCorrectly()
        {
            var dto = CreateSamplePropertyDTO();
            var entity = CreateSamplePropertyEntity();

            _propertyRepositoryMock.Setup(r => r.GetByIdWithInclude(dto.Id, It.IsAny<List<string>>()))
                .ReturnsAsync(entity);
            _featureRepositoryMock.Setup(f => f.GetById(It.IsAny<int>()))
                .ReturnsAsync((int id) => entity.Features.FirstOrDefault(f => f.Id == id));
            _propertyRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Property>()))
                .ReturnsAsync((Property p) => p);

            var result = await _service.UpdateAsync(dto, dto.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Price, result!.Price);
            Assert.Equal(dto.NumberOfRooms, result.NumberOfRooms);
        }

        [Fact]
        public async Task GenerateUniquePropertyCodeAsync_ShouldReturnCode()
        {
            _propertyRepositoryMock.Setup(r => r.GetByPropertyCodeAsync(It.IsAny<string>()))
                .ReturnsAsync((Property?)null);

            var code = await _service.GenerateUniquePropertyCodeAsync();

            Assert.NotNull(code);
            Assert.True(code.Length > 0);
        }

        [Fact]
        public async Task GetAgentIdByPropertyIdAsync_ShouldReturnAgentId()
        {
            var entity = CreateSamplePropertyEntity();
            _propertyRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(entity);

            var agentId = await _service.GetAgentIdByPropertyIdAsync(1);

            Assert.Equal(entity.AgentId, agentId);
        }
    }
}
