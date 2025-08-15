using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Queries.GetById
{
    /// <summary>
    /// Query para obtener un tipo de propiedad por Id.
    /// </summary>
    public class GetByIdPropertyTypeQuery : IRequest<PropertyTypeDTO?>
    {
        /// <summary>
        /// Id del tipo de propiedad.
        /// </summary>
        [SwaggerSchema(Description = "Id del tipo de propiedad a consultar", Nullable = false)]
        public int Id { get; set; }
    }

    public class GetByIdPropertyTypeQueryHandler : IRequestHandler<GetByIdPropertyTypeQuery, PropertyTypeDTO?>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdPropertyTypeQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PropertyTypeDTO?> Handle(GetByIdPropertyTypeQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;
            return _mapper.Map<PropertyTypeDTO>(entity);
        }
    }
}
