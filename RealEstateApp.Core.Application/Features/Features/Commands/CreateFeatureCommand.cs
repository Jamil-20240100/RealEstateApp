using AutoMapper;
using MediatR;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Features.Commands
{
    public class CreateFeatureCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
    }

    public class CreateFeatureCommandHandler : IRequestHandler<CreateFeatureCommand, int>
    {
        private readonly IFeatureRepository _repository;
        private readonly IMapper _mapper;

        public CreateFeatureCommandHandler(IFeatureRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateFeatureCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Feature>(request);
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
}
