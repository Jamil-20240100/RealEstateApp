using MediatR;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Commands.Delete
{

    public class DeleteSalesTypeCommand : IRequest<bool>
    {
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
            var salesType = await _repository.GetByIdAsync(request.Id);
            if (salesType == null) return false;

            await _repository.DeleteAsync(salesType);
            return true;
        }
    }
}
