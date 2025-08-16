using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Features.Features.Commands.Create;
using RealEstateApp.Core.Application.Features.Features.Commands.Delete;
using RealEstateApp.Core.Application.Features.Features.Commands.Update;
using RealEstateApp.Core.Application.Features.Features.Queries.GetById;
using RealEstateApp.Core.Application.Features.Features.Queries.List;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [SwaggerTag("Endpoints para la gestión de mejoras (features) en el sistema")]
    public class FeatureController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crear una nueva mejora",
            Description = "Crea una nueva mejora (feature) y devuelve el ID generado")]
        public async Task<IActionResult> Create([FromBody] CreateFeatureCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Actualizar una mejora existente",
            Description = "Actualiza una mejora según el ID especificado")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFeatureCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != command.Id)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");

            var result = await Mediator.Send(command);
            return result ? Ok(command) : NotFound();
        }

        [Authorize(Roles = "Admin, Developer")]
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<FeatureDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Listar todas las mejoras",
            Description = "Obtiene un listado con todas las mejoras registradas")]
        public async Task<IActionResult> List()
        {
            var features = await Mediator.Send(new ListFeaturesQuery());
            if (features == null || features.Count == 0)
                return NoContent();

            return Ok(features);
        }

        [Authorize(Roles = "Admin, Developer")]
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Obtener una mejora por ID",
            Description = "Obtiene los detalles de una mejora específica usando su ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var feature = await Mediator.Send(new GetByIdFeatureQuery { Id = id });
            if (feature == null)
                return NoContent();

            return Ok(feature);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Eliminar una mejora",
            Description = "Elimina una mejora específica usando su ID")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteFeatureCommand { Id = id });
            return result ? NoContent() : NotFound();
        }
    }
}
