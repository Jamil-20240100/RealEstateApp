using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty
{
    public class GetAgentPropertyQuery : IRequest<IList<PropertyForApiDTO>>
    {
        public required string Id { get; set; }
    }

    public class GetAgentPropertyQueryHandler : IRequestHandler<GetAgentPropertyQuery, IList<PropertyForApiDTO>>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetAgentPropertyQueryHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<IList<PropertyForApiDTO>> Handle(GetAgentPropertyQuery query, CancellationToken cancellationToken)
        {
            var propertyEntities = await _propertyRepository.GetAllQueryWithInclude(["PropertyType", "SalesType", "Features" ])
                .Where(p => p.AgentId == query.Id)
                .ToListAsync(cancellationToken);

            if (propertyEntities == null || propertyEntities.Count == 0)
                throw new ArgumentException("Properties not found with this id");

            var properties = _mapper.Map<List<PropertyForApiDTO>>(propertyEntities);

            return properties;
        }
    }
}
