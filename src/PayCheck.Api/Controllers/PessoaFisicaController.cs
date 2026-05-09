namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provides API endpoints for managing individual persons.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PessoaFisicaController : ControllerBase
    {
        private readonly IPessoaFisicaService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="PessoaFisicaController"/> class.
        /// </summary>
        /// <param name="service">The individual person service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <see langword="null"/>.</exception>
        public PessoaFisicaController(IPessoaFisicaService service)
        {
            this._service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Deletes an individual person by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the individual person to be deleted.</param>
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
                if (this._service.Get(guid) is null)
                    return NotFound();

                this._service.Delete(guid);

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves an individual person by its unique identifier.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the individual person.</param>
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
                var data = this._service.Get(guid);

                return data is null ?
                    NotFound() :
                    Ok(data);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves all registered individual persons.
        /// </summary>
        /// <response code="200">Returns the list of individual persons.</response>
        /// <response code="204">No individual persons found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PessoaFisicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoasFisicas()
        {
            try
            {
                var data = this._service.GetAll();

                return data is null || !data.Any() ?
                    NoContent() :
                    Ok(data);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves individual persons whose birthdays fall within the specified period.
        /// </summary>
        /// <param name="periodoInicialString">Start date in MMdd format.</param>
        /// <param name="periodoFinalString">End date in MMdd format.</param>
        /// <response code="200">Returns the list of individuals with birthdays in the period.</response>
        /// <response code="204">No individuals with birthdays found.</response>
        /// <response code="400">Missing or invalid parameters.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("Aniversariantes")]
        [ProducesResponseType(typeof(IEnumerable<PessoaFisicaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAniversariantes(
            [FromQuery] string periodoInicialString,
            [FromQuery] string periodoFinalString)
        {
            if (string.IsNullOrWhiteSpace(periodoInicialString) ||
                string.IsNullOrWhiteSpace(periodoFinalString))
                return BadRequest();

            try
            {
                var data = this._service.GetAniversariantes(
                    periodoInicialString,
                    periodoFinalString);

                return data is null || !data.Any() ?
                    NoContent() :
                    Ok(data);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates a new individual person.
        /// </summary>
        /// <param name="createDto">Data of the individual person to be created.</param>
        /// <response code="201">Individual person successfully created.</response>
        /// <response code="400">Invalid or missing request data.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreatePessoaFisica([FromBody] PessoaFisicaRequestCreateDto? createDto)
        {
            if (createDto is null)
                return BadRequest();

            try
            {
                var data = this._service.SaveData(
                    createDto);

                return CreatedAtAction(
                    nameof(this.GetPessoaFisica),
                    new
                    {
                        guid = data.Guid
                    },
                    data);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Updates the data of an existing individual person.
        /// </summary>
        /// <param name="guid">Unique identifier (GUID) of the individual person to be updated.</param>
        /// <param name="updateDto">Updated data of the individual person.</param>
        /// <response code="200">Individual person successfully updated.</response>
        /// <response code="400">Invalid or missing request data.</response>
        /// <response code="404">Individual person not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("{guid}")]
        [ProducesResponseType(typeof(PessoaFisicaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdatePessoaFisica(Guid guid, [FromBody] PessoaFisicaRequestUpdateDto? updateDto)
        {
            if (updateDto is null)
                return BadRequest();

            try
            {
                if (this._service.Get(guid) is null)
                    return NotFound();

                updateDto.Guid = guid;

                var data = this._service.SaveData(
                    updateDto: updateDto);

                return Ok(
                    data);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}