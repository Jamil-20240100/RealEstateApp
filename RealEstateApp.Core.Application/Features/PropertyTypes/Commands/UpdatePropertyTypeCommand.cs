using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Update
{
    /// <summary>
    /// Comando para actualizar un tipo de propiedad existente.
    /// </summary>
    public class UpdatePropertyTypeCommand : IRequest<bool>
    {
        /// <summary>
        /// Id del tipo de propiedad.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de propiedad", Nullable = false)]
        public required int Id { get; set; }

        /// <summary>
        /// Nuevo nombre del tipo de propiedad.
        /// </summary>
        [SwaggerSchema(Description = "Nombre del tipo de propiedad", Nullable = false)]
        public required string Name { get; set; }

        /// <summary>
        /// Nueva descripción del tipo de propiedad.
        /// </summary>
        [SwaggerSchema(Description = "Descripción del tipo de propiedad", Nullable = false)]
        public required string Description { get; set; }
    }

    public class UpdatePropertyTypeCommandHandler : IRequestHandler<UpdatePropertyTypeCommand, bool>
    {
        private readonly IPropertyTypeRepository _repository;

        public UpdatePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdatePropertyTypeCommand request, CancellationToken cancellationToken)
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
