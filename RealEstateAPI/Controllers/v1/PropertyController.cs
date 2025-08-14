using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Property;
using RealEstateApp.Core.Application.Features.Properties.Queries.GetById;
using RealEstateApp.Core.Application.Features.Properties.Queries.GetByCode;
using Swashbuckle.AspNetCore.Annotations;
using RealEstateAPI.Controllers;
using RealEstateApp.Core.Application.Features.Properties.Queries.List;

namespace RealEstateApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin, Developer")]
    [SwaggerTag("Endpoints to manage properties and retrieve property information")]
    public class PropertyController : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<PropertyForApiDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "List all properties",
            Description = "Retrieves a list of all properties registered in the system"
        )]
        public async Task<IActionResult> List()
        {
            try
            {
                var properties = await Mediator.Send(new ListPropertiesQuery());

                if (properties == null || properties.Count == 0)
                    return NoContent();

                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyForApiDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get property by ID",
            Description = "Retrieves the details of a property using its integer ID"
        )]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var property = await Mediator.Send(new GetByIdPropertyQuery { Id = id });

                if (property == null)
                    return NoContent();

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("by-code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyForApiDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get property by code",
            Description = "Retrieves the details of a property using its unique code"
        )]
        public async Task<IActionResult> GetByCode(string code)
        {
            try
            {
                var property = await Mediator.Send(new GetByCodePropertyQuery { Code = code });

                if (property == null)
                    return NoContent();

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
