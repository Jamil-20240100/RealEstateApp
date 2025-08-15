using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Features.Queries.List
{
    /// <summary>
    /// Query para listar todas las mejoras registradas.
    /// </summary>
    public class ListFeaturesQuery : IRequest<IList<FeatureDTO>> { }

    public class ListFeaturesQueryHandler : IRequestHandler<ListFeaturesQuery, IList<FeatureDTO>>
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;

        public ListFeaturesQueryHandler(IFeatureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<FeatureDTO>> Handle(ListFeaturesQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllQuery().ToListAsync(cancellationToken);
            return _mapper.Map<List<FeatureDTO>>(list);
        }
    }
}
