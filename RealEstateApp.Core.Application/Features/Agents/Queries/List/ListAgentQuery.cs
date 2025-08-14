using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.List
{
    /// <summary>
    /// Query parameters for retrieving all agents
    /// </summary>
    public class ListAgentQuery : IRequest<IList<AgentForApiDTO>>
    {
    }

    public class ListAgentQueryHandler : IRequestHandler<ListAgentQuery, IList<AgentForApiDTO>>
    {
        private readonly IAccountServiceForWebApi _accountServiceForWebApi;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public ListAgentQueryHandler(IPropertyRepository propertyRepository, IMapper mapper, IAccountServiceForWebApi accountServiceForWebApi)
        {
            _accountServiceForWebApi = accountServiceForWebApi;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<IList<AgentForApiDTO>> Handle(ListAgentQuery query, CancellationToken cancellationToken)
        {
            var agents = await _accountServiceForWebApi.GetAllUserByRole(Roles.Agent.ToString());

            var properties = await _propertyRepository
                .GetAllQuery()
                .Where(p => !string.IsNullOrEmpty(p.AgentId))
                .ToListAsync(cancellationToken);

            var propertyCountByAgent = properties
                .GroupBy(p => p.AgentId!.ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.Count());

            var agentDtos = agents.Select(a => new AgentForApiDTO
            {
                Id = a.Id,
                Name = a.Name,
                LastName = a.LastName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                NumberOfProperties = properties.Where(p => p.AgentId == a.Id).Count()
            }).ToList();

            return agentDtos;
        }
    }
}
