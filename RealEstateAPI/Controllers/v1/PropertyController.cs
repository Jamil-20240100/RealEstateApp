using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode;
using RealEstateApp.Core.Application.Features.Properties.Queries.GetById;
using RealEstateApp.Core.Application.Features.Properties.Queries.List;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, Developer")]
    [SwaggerTag("Endpoints to manage properties and retrieve property information")]
    public class PropertyController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<PropertyForApiDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "List all properties",
            Description = "Retrieves a list of all properties registered in the system"
        )]
        public async Task<IActionResult> List()
        {
            var properties = await Mediator.Send(new ListPropertiesQuery());

            if (properties == null || properties.Count == 0)
                return NoContent();

            return Ok(properties);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyForApiDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Get property by ID",
            Description = "Retrieves the details of a property using its integer ID"
        )]
        public async Task<IActionResult> GetById(int id)
        {
            var property = await Mediator.Send(new GetByIdPropertyQuery { Id = id });

            if (property == null)
                return NoContent();

            return Ok(property);
        }

        [HttpGet("by-code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyForApiDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [SwaggerOperation(
            Summary = "Get property by code",
            Description = "Retrieves the details of a property using its unique code"
        )]
        public async Task<IActionResult> GetByCode(string code)
        {
            var property = await Mediator.Send(new GetByCodePropertyQuery { Code = code });

            if (property == null)
                return NoContent();

            return Ok(property);
        }
    }
}