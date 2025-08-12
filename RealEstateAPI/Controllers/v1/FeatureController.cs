using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.DTOs.Feature;
using RealEstateApp.Core.Application.Features.Features.Commands;
using RealEstateApp.Core.Application.Features.Features.Commands.Delete;
using RealEstateApp.Core.Application.Features.Features.Commands.Update;
using RealEstateApp.Core.Application.Features.Features.Queries.GetById;
using RealEstateApp.Core.Application.Features.Features.Queries.List;

namespace RealEstateAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    [Authorize]
    public class FeatureController : BaseApiController
    {

        /// <summary>
        /// Crear una nueva mejora
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateFeatureCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var id = await Mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Actualizar una mejora existente
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateFeatureCommand command)
        {
            try
            {
                if (id != command.Id)
                    return BadRequest("El ID de la URL no coincide con el ID del cuerpo.");

                var result = await Mediator.Send(command);

                return result ? Ok(command) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        /// <summary>
        /// Listar todas las mejoras
        /// </summary>
        [Authorize(Roles = "Admin, Developer")]
        [HttpGet("list")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<FeatureDTO>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> List()
        {
            try
            {
                var features = await Mediator.Send(new ListFeaturesQuery());

                if (features == null || features.Count == 0)
                    return NoContent();

                return Ok(features);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Obtener una mejora por Id
        /// </summary>
        [Authorize(Roles = "Admin, Developer")]
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeatureDTO))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var feature = await Mediator.Send(new GetByIdFeatureQuery { Id = id });

                if (feature == null)
                    return NoContent();

                return Ok(feature);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }





        /// <summary>
        /// Eliminar una mejora
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await Mediator.Send(new DeleteFeatureCommand { Id = id });
                return result ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
