using MediatR;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.SalesType.Commands.Delete
{
    /// <summary>
    /// Comando para eliminar un tipo de venta.
    /// </summary>
    public class DeleteSalesTypeCommand : IRequest<bool>
    {
        /// <summary>
        /// Id del tipo de venta a eliminar.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de venta", Nullable = false)]
        public int Id { get; set; }
    }

    public class DeleteSalesTypeCommandHandler : IRequestHandler<DeleteSalesTypeCommand, bool>
    {
        private readonly ISalesTypeRepository _repository;

        public DeleteSalesTypeCommandHandler(ISalesTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteSalesTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetById(request.Id);
            if (entity == null) return false;

            await _repository.DeleteAsync(entity);
            return true;
        }
    }
}
