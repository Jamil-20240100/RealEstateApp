using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Update;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Delete;
using RealEstateApp.Core.Application.Features.SalesTypes.Queries.GetById;
using RealEstateApp.Core.Application.Features.SalesType.Queries.List;
using Swashbuckle.AspNetCore.Annotations;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    [SwaggerTag("Endpoints para la gestión de tipos de venta en el sistema")]
    public class SalesTypeController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crear un tipo de venta",
            Description = "Crea un nuevo tipo de venta y devuelve el ID generado")]
        public async Task<IActionResult> Create([FromBody] CreateSalesTypeCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SalesTypeDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Actualizar un tipo de venta",
            Description = "Actualiza un tipo de venta existente según su ID")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSalesTypeCommand command)
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SalesTypeDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Listar todos los tipos de venta",
            Description = "Obtiene un listado con todos los tipos de venta registrados")]
        public async Task<IActionResult> List()
        {
            var list = await Mediator.Send(new ListSalesTypesQuery());
            if (list == null || list.Count == 0)
                return NoContent();

            return Ok(list);
        }

        [Authorize(Roles = "Admin,Developer")]
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SalesTypeDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Obtener un tipo de venta por ID",
            Description = "Obtiene los detalles de un tipo de venta usando su ID")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await Mediator.Send(new GetByIdSalesTypeQuery { Id = id });
            return entity == null ? NoContent() : Ok(entity);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Eliminar un tipo de venta",
            Description = "Elimina un tipo de venta específico usando su ID")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteSalesTypeCommand { Id = id });
            return result ? NoContent() : NotFound();
        }
    }
}
