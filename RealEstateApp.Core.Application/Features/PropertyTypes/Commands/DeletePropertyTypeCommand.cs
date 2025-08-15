using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Delete
{
    /// <summary>
    /// Comando para eliminar un tipo de propiedad.
    /// </summary>
    public class DeletePropertyTypeCommand : IRequest<bool>
    {
        /// <summary>
        /// Id del tipo de propiedad a eliminar.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de propiedad", Nullable = false)]
        public int Id { get; set; }
    }

    public class DeletePropertyTypeCommandHandler : IRequestHandler<DeletePropertyTypeCommand, bool>
    {
        private readonly IPropertyTypeRepository _repository;

        public DeletePropertyTypeCommandHandler(IPropertyTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }
}
