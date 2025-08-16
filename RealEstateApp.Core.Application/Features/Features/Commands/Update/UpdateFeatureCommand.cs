using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.Features.Commands.Update
{
    /// <summary>
    /// Comando para actualizar los datos de una mejora existente.
    /// </summary>
    public class UpdateFeatureCommand : IRequest<bool>
    {
        /// <summary>
        /// Id de la mejora a actualizar.
        /// </summary>
        [SwaggerSchema(Description = "Id de la mejora", Nullable = false)]
        public required int Id { get; set; }

        /// <summary>
        /// Nuevo nombre de la mejora.
        /// </summary>
        [SwaggerSchema(Description = "Nombre de la mejora", Nullable = false)]
        public required string Name { get; set; }

        /// <summary>
        /// Nueva descripción de la mejora.
        /// </summary>
        [SwaggerSchema(Description = "Descripción de la mejora", Nullable = false)]
        public required string Description { get; set; }
    }

    public class UpdateFeatureCommandHandler : IRequestHandler<UpdateFeatureCommand, bool>
    {
        private readonly IFeatureRepository _repository;

        public UpdateFeatureCommandHandler(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateFeatureCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Description = request.Description;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
