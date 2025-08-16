using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.Features.Commands.Delete
{
    /// <summary>
    /// Comando para eliminar una mejora existente.
    /// </summary>
    public class DeleteFeatureCommand : IRequest<bool>
    {
        /// <summary>
        /// Identificador único de la mejora.
        /// </summary>
        [SwaggerSchema(Description = "Id de la mejora a eliminar", Nullable = false)]
        public int Id { get; set; }
    }

    public class DeleteFeatureCommandHandler : IRequestHandler<DeleteFeatureCommand, bool>
    {
        private readonly IFeatureRepository _repository;

        public DeleteFeatureCommandHandler(IFeatureRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteFeatureCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }
}
