using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.PropertyType;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Create;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Delete;
using RealEstateApp.Core.Application.Features.PropertyTypes.Commands.Update;
using RealEstateApp.Core.Application.Features.PropertyTypes.Queries.GetById;
using RealEstateApp.Core.Application.Features.PropertyTypes.Queries.List;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [SwaggerTag("Endpoints para la gestión de tipos de propiedades en el sistema")]
    public class PropertyTypeController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crear un tipo de propiedad",
            Description = "Crea un nuevo tipo de propiedad y devuelve el ID generado")]
        public async Task<IActionResult> Create([FromBody] CreatePropertyTypeCommand command)
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
            Summary = "Actualizar un tipo de propiedad",
            Description = "Actualiza un tipo de propiedad existente según su ID")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyTypeCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != command.Id)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");

            var result = await Mediator.Send(command);
            return result ? Ok(command) : NotFound();
        }

        [Authorize(Roles = "Admin,Developer")]
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<PropertyTypeDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Listar todos los tipos de propiedades",
            Description = "Obtiene un listado con todos los tipos de propiedades registradas")]
        public async Task<IActionResult> List()
        {
            var list = await Mediator.Send(new ListPropertyTypesQuery());
            if (list == null || list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [Authorize(Roles = "Admin,Developer")]
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyTypeDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Obtener un tipo de propiedad por ID",
            Description = "Obtiene los detalles de un tipo de propiedad usando su ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await Mediator.Send(new GetByIdPropertyTypeQuery { Id = id });
            return entity == null ? NoContent() : Ok(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Eliminar un tipo de propiedad",
            Description = "Elimina un tipo de propiedad específico usando su ID")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeletePropertyTypeCommand { Id = id });
            return result ? NoContent() : NotFound();
        }
    }
}
