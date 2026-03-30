namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly IPessoaJuridicaService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaJuridicaController(IPessoaJuridicaService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// Deletes a legal entity by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the legal entity to be deleted.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Legal entity successfully deleted.</response>
        /// <response code="404">Legal entity not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePessoaJuridica(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Jurídica {guid} não encontrada.",
                        });

                this._service.Delete(
                    guid);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ex.Message,
                    });
            }
        }

        /// <summary>
        /// Retrieves a legal entity by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the legal entity.</param>
        /// <returns>Data of the found legal entity.</returns>
        /// <response code="200">Returns the requested legal entity.</response>
        /// <response code="404">Legal entity not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{guid}")]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoaJuridica(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Jurídica {guid} não encontrada.",
                        });

                return Ok(
                    data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ex.Message,
                    });
            }
        }

        /// <summary>
        /// Retrieves all registered legal entities.
        /// </summary>
        /// <returns>List of legal entities.</returns>
        /// <response code="200">Returns the list of legal entities.</response>
        /// <response code="404">No legal entities found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PessoaJuridicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoasJuridicas()
        {
            try
            {
                var data = this._service.GetAll();

                if (data is null ||
                    !data.Any())
                    return NotFound(
                        new
                        {
                            Message = "Não há registros de Pessoas Jurídicas.",
                        });

                return Ok(
                    data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ex.Message,
                    });
            }
        }

        /// <summary>
        /// Creates a new legal entity.
        /// </summary>
        /// <remarks>
        /// This operation creates a new legal entity record with the data provided
        /// in the request body. On success, it returns the created entity data along
        /// with the Location header containing the URL of the new resource.
        /// </remarks>
        /// <param name="createDto">Data of the legal entity to be created.</param>
        /// <returns>Data of the created legal entity.</returns>
        /// <response code="201">Legal entity successfully created.</response>
        /// <response code="400">Invalid data or malformed request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        public IActionResult CreatePessoaJuridica([FromBody] PessoaJuridicaRequestCreateDto createDto)
        {
            if (createDto is null)
                return BadRequest(
                    new
                    {
                        Message = "Os dados da Pessoa Jurídica são obrigatórios.",
                    });

            try
            {
                var data = this._service.SaveData(
                    createDto);

                return CreatedAtAction(
                    nameof(
                        this.GetPessoaJuridica),
                    new
                    {
                        guid = data.Guid,
                    },
                    data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ex.Message,
                    });
            }
        }

        /// <summary>
        /// Updates the data of an existing legal entity.
        /// </summary>
        /// <remarks>
        /// This operation replaces the existing data of the specified legal entity
        /// with the values provided in the request body. The GUID in the route is used
        /// to identify the entity to update.
        /// </remarks>
        /// <param name="guid">Unique identifier (GUID) of the legal entity to be updated.</param>
        /// <param name="updateDto">Updated data of the legal entity.</param>
        /// <returns>Data of the updated legal entity.</returns>
        /// <response code="200">Legal entity successfully updated.</response>
        /// <response code="400">Invalid data or malformed request.</response>
        /// <response code="404">Legal entity not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{guid}")]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePessoaJuridica(Guid guid, [FromBody] PessoaJuridicaRequestUpdateDto updateDto)
        {
            if (updateDto is null)
                return BadRequest(
                    new
                    {
                        Message = "Os dados da Pessoa Jurídica são obrigatórios.",
                    });

            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Jurídica {guid} não encontrada.",
                        });

                updateDto.Guid = guid;

                data = this._service.SaveData(
                    updateDto: updateDto);

                return Ok(
                    data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        ex.Message,
                    });
            }
        }
    }
}