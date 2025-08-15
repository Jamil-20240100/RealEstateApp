using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Queries.GetById
{
    public class GetByIdSalesTypeQuery : IRequest<SalesTypeDTO>
    {
        public int Id { get; set; }
    }
    public class GetByIdSalesTypeQueryHandler : IRequestHandler<GetByIdSalesTypeQuery, SalesTypeDTO>
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSalesTypeQueryHandler(ISalesTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SalesTypeDTO> Handle(GetByIdSalesTypeQuery request, CancellationToken cancellationToken)
        {
            var salesType = await _repository.GetById(request.Id);
            return _mapper.Map<SalesTypeDTO>(salesType);
        }
    }
}
