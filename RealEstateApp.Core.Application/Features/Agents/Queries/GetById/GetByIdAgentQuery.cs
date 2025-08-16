using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Exceptions;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.GetById
{
    /// <summary>
    /// Query to retrieve an agent by their unique ID
    /// </summary>
    public class GetByIdAgentQuery : IRequest<AgentForApiDTO>
    {
        /// <summary>
        /// The unique identifier of the agent
        /// </summary>
        /// <example>5ac0dfee-9c1c-4f9e-bdd3-5a2d864faea2</example>
        [SwaggerParameter(Description = "The ID of the agent to retrieve")]
        public required string Id { get; set; }
    }

    public class GetByIdAgentQueryHandler : IRequestHandler<GetByIdAgentQuery, AgentForApiDTO>
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public GetByIdAgentQueryHandler(IAccountServiceForWebApi accountServiceForWebApi, IMapper mapper, IPropertyRepository propertyRepository)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<AgentForApiDTO> Handle(GetByIdAgentQuery query, CancellationToken cancellationToken)
        {
            var user = await _accountServiceForWebApi.GetUserById(query.Id);

            if (user == null)
                throw new ApiException("Agent not found with this id", (int)HttpStatusCode.NotFound);

            var agent = _mapper.Map<AgentForApiDTO>(user);

            var properties = await _propertyRepository
                .GetAllQuery()
                .Where(p => p.AgentId == agent.Id)
                .ToListAsync(cancellationToken);

            agent.NumberOfProperties = properties.Count;

            return agent;
        }

    }
}
