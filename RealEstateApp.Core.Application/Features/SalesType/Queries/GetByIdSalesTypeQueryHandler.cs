using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.SalesTypes.Queries.GetById
{
    /// <summary>
    /// Query para obtener un tipo de venta por Id.
    /// </summary>
    public class GetByIdSalesTypeQuery : IRequest<SalesTypeDTO?>
    {
        /// <summary>
        /// Id del tipo de venta.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de venta a consultar", Nullable = false)]
        public int Id { get; set; }
    }

    public class GetByIdSalesTypeQueryHandler : IRequestHandler<GetByIdSalesTypeQuery, SalesTypeDTO?>
    {
        private readonly ISalesTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSalesTypeQueryHandler(ISalesTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SalesTypeDTO?> Handle(GetByIdSalesTypeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetById(request.Id);
            if (entity == null) return null;
            return _mapper.Map<SalesTypeDTO>(entity);
        }
    }
}
