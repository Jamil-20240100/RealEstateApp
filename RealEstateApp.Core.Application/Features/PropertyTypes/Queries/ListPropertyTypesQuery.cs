using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.PropertyTypes.Queries.List
{
    public class ListPropertyTypesQuery : IRequest<IList<PropertyTypeDTO>> { }

    public class ListPropertyTypesQueryHandler : IRequestHandler<ListPropertyTypesQuery, IList<PropertyTypeDTO>>
    {
        private readonly IPropertyTypeRepository _repository;
        private readonly IMapper _mapper;

        public ListPropertyTypesQueryHandler(IPropertyTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IList<PropertyTypeDTO>> Handle(ListPropertyTypesQuery request, CancellationToken cancellationToken)
        {
            var list = await _repository.GetAllQuery().ToListAsync(cancellationToken);
            return _mapper.Map<List<PropertyTypeDTO>>(list);
        }
    }
}
