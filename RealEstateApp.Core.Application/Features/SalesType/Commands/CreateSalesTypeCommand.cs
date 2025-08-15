using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create
{
    /// <summary>
    /// Comando para crear un nuevo tipo de venta.
    /// </summary>
    public class CreateSalesTypeCommand : IRequest<int>
    {
        /// <summary>
        /// Nombre del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Nombre del tipo de venta", Nullable = false)]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Descripción del tipo de venta", Nullable = false)]
        public required string Description { get; set; }
    }

    public class CreateSalesTypeCommandHandler : IRequestHandler<CreateSalesTypeCommand, int>
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;

        public CreateSalesTypeCommandHandler(ISalesTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateSalesTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<SalesType>(request);
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
}
