using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Domain.Interfaces;
using System.Net;

namespace RealEstateApp.Core.Application.Features.SalesType.Queries.List
{
    public class ListSalesTypesQuery : IRequest<IList<SalesTypeDTO>> { }

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
            var list = await _repository.GetAllQuery().ToListAsync(cancellationToken);
            if (list == null || !list.Any())
                throw new ApiException("No sales types found", (int)HttpStatusCode.NotFound);

            return _mapper.Map<List<SalesTypeDTO>>(list);
        }
    }
}
