using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.SalesType.Commands.Update
{
    /// <summary>
    /// Comando para actualizar un tipo de venta existente.
    /// </summary>
    public class UpdateSalesTypeCommand : IRequest<bool>
    {
        /// <summary>
        /// Id del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de venta", Nullable = false)]
        public required int Id { get; set; }

        /// <summary>
        /// Nuevo nombre del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Nombre del tipo de venta", Nullable = false)]
        public required string Name { get; set; }

        /// <summary>
        /// Nueva descripción del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Descripción del tipo de venta", Nullable = false)]
        public required string Description { get; set; }
    }

    public class UpdateSalesTypeCommandHandler : IRequestHandler<UpdateSalesTypeCommand, bool>
    {
        private readonly ISalesTypeRepository _repository;

        public UpdateSalesTypeCommandHandler(ISalesTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(UpdateSalesTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetById(request.Id);
            if (entity == null) return false;

            entity.Name = request.Name;
            entity.Description = request.Description;

            await _repository.UpdateAsync(entity);
            return true;
        }
    }
}
