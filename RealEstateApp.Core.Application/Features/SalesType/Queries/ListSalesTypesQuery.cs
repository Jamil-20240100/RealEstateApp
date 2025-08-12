using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Queries.List
{

    public class ListSalesTypesQuery : IRequest<IList<SalesTypeDTO>>
    {
    }

    public class ListSalesTypesQueryHandler : IRequestHandler<ListSalesTypesQuery, IList<SalesTypeDTO>>
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;

        public ListSalesTypesQueryHandler(ISalesTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<SalesTypeDTO>> Handle(ListSalesTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllAsync();
            return _mapper.Map<IList<SalesTypeDTO>>(list);
        }
    }
}
