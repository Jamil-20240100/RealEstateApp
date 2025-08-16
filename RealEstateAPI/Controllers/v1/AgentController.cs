using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Agent;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Features.Agents.Commands.ChangeStatus;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetAgentProperty;
using RealEstateApp.Core.Application.Features.Agents.Queries.GetById;
using RealEstateApp.Core.Application.Features.Agents.Queries.List;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, Developer")]
    [SwaggerTag("Endpoints to manage agents, their properties, and status updates")]
    public class AgentController : BaseApiController
    {
        private readonly IMediator _mediator;

        public AgentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AgentForApiDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "List all agents",
            Description = "Retrieves a list of all agents registered in the system"
        )]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new ListAgentQuery());

            if (result == null || !result.Any())
                return NoContent();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentForApiDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Get agent by ID",
            Description = "Retrieves the details of a single agent by their unique identifier"
        )]
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
        [SwaggerOperation(
            Summary = "Get properties of an agent",
            Description = "Retrieves all properties associated with a specific agent"
        )]
        public async Task<IActionResult> GetAgentProperties(string id)
        {
            var result = await _mediator.Send(new GetAgentPropertyQuery { Id = id });

            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{userId}/change-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Change agent status",
            Description = "Updates the active status (enabled/disabled) of an agent"
        )]
        public async Task<IActionResult> ChangeUserStatus(string userId, [FromBody] ChangeStatusAgentCommand command)
        {
            if (userId != command.UserId)
            {
                return BadRequest("The ID in the URL does not match the request body.");
            }

            await _mediator.Send(command);

            return NoContent();
        }
    }
}
