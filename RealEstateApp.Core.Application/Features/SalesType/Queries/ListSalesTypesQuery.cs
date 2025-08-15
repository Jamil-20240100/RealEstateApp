using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Queries.List
{
    /// <summary>
    /// Query para listar todos los tipos de ventas.
    /// </summary>
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
            return _mapper.Map<List<SalesTypeDTO>>(list);
        }
    }
}
