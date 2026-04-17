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
    public class PessoaFisicaController : ControllerBase
    {
        private readonly IPessoaFisicaService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaFisicaController(IPessoaFisicaService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// Deletes an individual person by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the individual person to be deleted.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Individual person successfully deleted.</response>
        /// <response code="404">Individual person not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePessoaFisica(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Física {guid} não encontrada.",
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
        /// Retrieves an individual person by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the individual person.</param>
        /// <returns>Data of the found individual person.</returns>
        /// <response code="200">Returns the requested individual person.</response>
        /// <response code="404">Individual person not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{guid}")]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoaFisica(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Física {guid} não encontrada.",
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
        /// Retrieves all registered individual persons.
        /// </summary>
        /// <returns>List of individual persons.</returns>
        /// <response code="200">Returns the list of individual persons.</response>
        /// <response code="404">No individual persons found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PessoaFisicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoasFisicas()
        {
            try
            {
                var data = this._service.GetAll();

                if (data is null ||
                    !data.Any())
                    return NotFound(
                        new
                        {
                            Message = "Não há registros de Pessoas Físicas.",
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
        /// Retrieves individual persons whose birthdays fall within the specified period.
        /// </summary>
        /// <param name="periodoInicialString">Start date in MMdd format.</param>
        /// <param name="periodoFinalString">End date in MMdd format.</param>
        /// <returns>List of individuals with birthdays in the specified period.</returns>
        /// <response code="200">Returns the list of individuals with birthdays.</response>
        /// <response code="404">No individuals with birthdays found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Aniversariantes")]
        [ProducesResponseType(typeof(IEnumerable<PessoaFisicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAniversariantes(
            [FromQuery] string periodoInicialString,
            [FromQuery] string periodoFinalString)
        {
            try
            {
                //  Validação dos parâmetros.
                if (string.IsNullOrWhiteSpace(periodoInicialString) ||
                    string.IsNullOrWhiteSpace(periodoFinalString))
                    return BadRequest(
                        new
                        {
                            Message = $"Os parâmetros {nameof(periodoInicialString)} e {nameof(periodoFinalString)} são obrigatórios.",
                        });

                //if (!IsValidMonthDay(periodoInicialString) || !IsValidMonthDay(periodoFinalString))
                //    return BadRequest(
                //        new 
                //        { 
                //            Message = "Formato de data inválido. Use o formato MM-dd (ex: 01-15).",
                //        });

                var data = this._service.GetAniversariantes(
                    periodoInicialString,
                    periodoFinalString);

                if (data is null ||
                    !data.Any())
                    return NotFound(
                        new
                        {
                            Message = $"Não há registros de Pessoas Físicas que realizam aniversário no período de {periodoInicialString} a {periodoFinalString}.",
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
        /// Creates a new individual person.
        /// </summary>
        /// <param name="createDto">Data of the individual person to be created.</param>
        /// <returns>Data of the created individual person.</returns>
        /// <response code="201">Individual person successfully created.</response>
        /// <response code="400">Invalid data or malformed request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePessoaFisica([FromBody] PessoaFisicaRequestCreateDto createDto)
        {
            if (createDto is null)
                return BadRequest(
                    new
                    {
                        Message = "Os dados da Pessoa Física são obrigatórios.",
                    });

            try
            {
                var data = this._service.SaveData(
                    createDto);

                return CreatedAtAction(
                    nameof(
                        this.GetPessoaFisica),
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
        /// Updates the data of an existing individual person identified by the specified GUID.
        /// </summary>
        /// <remarks>
        /// This operation replaces the existing data of the specified individual person with the values provided in
        /// the request body. The GUID in the route is used to identify the entity to update.
        /// </remarks>
        /// <param name="guid">Unique identifier (GUID) of the individual person to be updated.</param>
        /// <param name="updateDto">Updated data of the individual person.</param>
        /// <returns>Data of the updated individual person.</returns>
        /// <response code="200">Individual person successfully updated.</response>
        /// <response code="400">Invalid data or malformed request.</response>
        /// <response code="404">Individual person not found.</response>
        /// <response code="500">Internal server error.</response>
        /// <returns>
        /// An IActionResult that represents the result of the update operation. Returns status 200 (OK) with the updated
        /// individual person data if successful; status 400 (Bad Request) if the input data is invalid; status 404 (Not Found)
        /// if the specified individual person does not exist; or status 500 (Internal Server Error) if an unexpected error occurs.
        /// </returns>
        [HttpPut("{guid}")]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePessoaFisica(Guid guid, [FromBody] PessoaFisicaRequestUpdateDto updateDto)
        {
            if (updateDto is null)
                return BadRequest(
                    new
                    {
                        Message = "Os dados da Pessoa Física são obrigatórios.",
                    });

            try
            {
                var data = this._service.Get(
                    guid);

                if (data is null)
                    return NotFound(
                        new
                        {
                            Message = $"Pessoa Física {guid} não encontrada.",
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

        private static bool IsValidMonthDay(string value)
        {
            // Formato esperado: MM-dd (ex: 01-15, 12-31)
            if (value.Length != 5 || value[2] != '-')
                return false;

            return int.TryParse(value[..2], out int month)
                && int.TryParse(value[3..], out int day)
                && month >= 1 && month <= 12
                && day >= 1 && day <= 31;
        }
    }
}