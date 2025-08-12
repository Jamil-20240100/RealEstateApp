using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create
{
    public class CreateSalesTypeCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
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
            var salesType = _mapper.Map<SalesType>(request);
            salesType = await _repository.AddAsync(salesType);
            return salesType.Id;
        }
    }
}
