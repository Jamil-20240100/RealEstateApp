using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.SalesType;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Create;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Delete;
using RealEstateApp.Core.Application.Features.SalesTypes.Commands.Update;
using RealEstateApp.Core.Application.Features.SalesTypes.Queries.GetById;
using RealEstateApp.Core.Application.Features.SalesTypes.Queries.List;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class SalesTypeController : BaseApiController
    {
        /// <summary>
        /// Crear un tipo de venta
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateSalesTypeCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// Actualizar un tipo de venta
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SalesTypeDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSalesTypeCommand command)
        {
            if (id != command.Id) return BadRequest();
            var result = await Mediator.Send(command);
            return result ? Ok(command) : NotFound();
        }

        /// <summary>
        /// Listar todos los tipos de ventas
        /// </summary>
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<SalesTypeDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            var list = await Mediator.Send(new ListSalesTypesQuery());
            if (list == null || list.Count == 0) return NoContent();
            return Ok(list);
        }

        /// <summary>
        /// Obtener un tipo de venta por Id
        /// </summary>
        [Authorize(Roles = "Admin,Developer")]
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SalesTypeDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await Mediator.Send(new GetByIdSalesTypeQuery { Id = id });
            return entity == null ? NoContent() : Ok(entity);
        }

        /// <summary>
        /// Eliminar un tipo de venta
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await Mediator.Send(new DeleteSalesTypeCommand { Id = id });
            return result ? NoContent() : NotFound();
        }
    }
}
