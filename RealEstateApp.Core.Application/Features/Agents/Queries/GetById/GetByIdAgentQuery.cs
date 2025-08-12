using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Features.Agents.Queries.GetById
{
    public class GetByIdAgentQuery : IRequest<AgentForApiDTO>
    {
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
            var agent = _mapper.Map<AgentForApiDTO>(await _accountServiceForWebApi.GetUserById(query.Id));

            var properties = await _propertyRepository
                .GetAllQuery().Where(p => p.AgentId == agent.Id)
                .ToListAsync(cancellationToken);

            agent.NumberOfProperties = properties.Count;

            return agent;
        }
    }
}
