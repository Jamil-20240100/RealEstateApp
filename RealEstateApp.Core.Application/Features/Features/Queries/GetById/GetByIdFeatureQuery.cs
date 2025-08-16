using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealEstateApp.Core.Application.Features.Features.Queries.GetById
{
    /// <summary>
    /// Query para obtener los detalles de una mejora por su Id.
    /// </summary>
    public class GetByIdFeatureQuery : IRequest<FeatureDTO>
    {
        [SwaggerSchema(Description = "Id de la mejora a consultar", Nullable = false)]
        public int Id { get; set; }
    }

    public class GetByIdFeatureQueryHandler : IRequestHandler<GetByIdFeatureQuery, FeatureDTO>
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdFeatureQueryHandler(IFeatureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FeatureDTO> Handle(GetByIdFeatureQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null)
                throw new ApiException("Feature not found with this id", (int)HttpStatusCode.NotFound);

            return _mapper.Map<FeatureDTO>(entity);
        }
    }
}
