using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.Features.Commands.Create
{
    /// <summary>
    /// Comando para crear una nueva mejora.
    /// </summary>
    public class CreateFeatureCommand : IRequest<int>
    {
        /// <summary>
        /// Nombre de la mejora.
        /// </summary>
        [SwaggerSchema(Description = "Nombre de la mejora", Nullable = false)]
        public required string Name { get; set; }

        /// <summary>
        /// Descripción de la mejora.
        /// </summary>
        [SwaggerSchema(Description = "Descripción de la mejora", Nullable = false)]
        public required string Description { get; set; }
    }

    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand, int>
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;

        public CreateFeatureCommandHandler(IFeatureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Feature>(request);
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
}
