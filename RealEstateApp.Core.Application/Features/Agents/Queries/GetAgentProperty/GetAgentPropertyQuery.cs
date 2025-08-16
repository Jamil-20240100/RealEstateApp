using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty
{
    /// <summary>
    /// Query to retrieve all properties of a specific agent
    /// </summary>
    public class GetAgentPropertyQuery : IRequest<IList<PropertyForApiDTO>>
    {
        /// <summary>
        /// The unique identifier of the agent
        /// </summary>
        /// <example>5ac0dfee-9c1c-4f9e-bdd3-5a2d864faea2</example>
        [SwaggerParameter(Description = "The ID of the agent whose properties are being requested")]
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
            var propertyEntities = await _propertyRepository.GetAllQueryWithInclude([ "PropertyType", "SalesType", "Features" ])
                .Where(p => p.AgentId == query.Id)
                .ToListAsync(cancellationToken);

            if (!propertyEntities.Any()) throw new ApiException("Properties not found with this id", (int)HttpStatusCode.NotFound);

            var properties = _mapper.Map<List<PropertyForApiDTO>>(propertyEntities);

            return properties;
        }
    }
}
