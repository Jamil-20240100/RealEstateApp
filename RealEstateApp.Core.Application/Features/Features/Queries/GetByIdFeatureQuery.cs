using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Features.Queries.GetById
{
    public class GetByIdFeatureQuery : IRequest<FeatureDTO?>
    {
        public int Id { get; set; }
    }

    public class GetByIdFeatureQueryHandler : IRequestHandler<GetByIdFeatureQuery, FeatureDTO?>
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdFeatureQueryHandler(IFeatureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<FeatureDTO?> Handle(GetByIdFeatureQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            if (entity == null) return null;
            return _mapper.Map<FeatureDTO>(entity);
        }
    }
}
