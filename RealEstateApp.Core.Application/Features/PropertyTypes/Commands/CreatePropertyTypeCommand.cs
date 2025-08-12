using AutoMapper;
using MediatR;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Create
{
    public class CreatePropertyTypeCommand : IRequest<int>
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
    }

    public class CreatePropertyTypeCommandHandler : IRequestHandler<CreatePropertyTypeCommand, int>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public CreatePropertyTypeCommandHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreatePropertyTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<PropertyType>(request);
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
}
