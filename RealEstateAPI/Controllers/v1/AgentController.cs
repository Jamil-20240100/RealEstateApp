using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetById;
using RealEstateApp.Core.Application.Features.Agents.Queries.List;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using RealEstateApp.Core.Application.Features.Agents.Commands.ChangeStatus;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, Developer")]
    public class AgentController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AgentForApiDTO>))]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new ListAgentQuery());
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentForApiDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetByIdAgentQuery { Id = id });

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}/agent-properties")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<PropertyForApiDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAgentProperties(string id)
        {
            var result = await _mediator.Send(new GetAgentPropertyQuery { Id = id });

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{userId}/change-status")]
        public async Task<IActionResult> ChangeUserStatus(string userId, [FromBody] bool newStatus)
        {
            try
            {
                var result = await _mediator.Send(new ChangeStatusAgentCommand
                {
                    UserId = userId,
                    NewStatus = newStatus
                });
            }catch(Exception)
            {
                return BadRequest("Failed to update user status");
            }
            return NoContent();
        }

    }
}
