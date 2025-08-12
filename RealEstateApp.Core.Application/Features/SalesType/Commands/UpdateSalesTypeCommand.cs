using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Commands.Update
{

    public class UpdateSalesTypeCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateSalesTypeCommandHandler : IRequestHandler<UpdateSalesTypeCommand, bool>
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSalesTypeCommandHandler(ISalesTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateSalesTypeCommand request, CancellationToken cancellationToken)
        {
            var salesType = await _repository.GetByIdAsync(request.Id);
            if (salesType == null) return false;

            salesType.Name = request.Name;
            salesType.Description = request.Description;

            await _repository.UpdateAsync(salesType);
            return true;
        }
    }
}
