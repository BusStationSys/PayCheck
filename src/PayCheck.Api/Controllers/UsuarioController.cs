namespace PayCheck.Api.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller responsible for user-related operations.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        /// <summary>
        /// Initializes a new instance of <see cref="UsuarioController"/>.
        /// </summary>
        /// <param name="service">The user service instance.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is null.</exception>
        public UsuarioController(IUsuarioService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="loginRequest">The login credentials.</param>
        /// <returns>The authenticated user data.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest loginRequest)
        {
            var data = await Task.FromResult(
                this._service.GetByUsername(
                    loginRequest.CpfEmailUsername).FirstOrDefault());

            if (data is null)
                return Unauthorized(
                    new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Detail = $"Usuário não encontrado para a credencial {loginRequest.CpfEmailUsername}!",
                    });

            data = await Task.FromResult(
                this._service.CheckPasswordValid(
                    data.Guid,
                    loginRequest.Password));

            if (data is null)
                return Unauthorized(
                    new ProblemDetails
                    {
                        Title = "Unauthorized",
                        Detail = $"A Senha não confere para o Usuário com a credencial {loginRequest.CpfEmailUsername}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves the notifications for a given user.
        /// </summary>
        /// <param name="guid">The user unique identifier.</param>
        /// <returns>The list of notifications for the user.</returns>
        [HttpGet("Notificacoes/{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotificacoesAsync(Guid guid)
        {
            var data = await Task.FromResult(
                this._service.GetNotificacoes(
                    guidUsuario: guid));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Notificações não encontradas para o Usuário {guid}.",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Updates the data of an existing user.
        /// </summary>
        /// <param name="guid">The user unique identifier.</param>
        /// <param name="updateRequest">The update payload.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUsuarioAsync(Guid guid, [FromBody] UsuarioUpdateRequest updateRequest)
        {
            if (updateRequest is null)
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Payload inválido."
                    });

            updateRequest.Guid = guid;

            var data = await Task.FromResult(
                this._service.SaveData(
                    updateRequest: updateRequest));

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Usuário {guid} não encontrado.",
                    });

            return NoContent();
        }

        /// <summary>
        /// Updates the password of an existing user.
        /// </summary>
        /// <param name="guid">The user unique identifier.</param>
        /// <param name="updateRequest">The update payload containing the new password.</param>
        /// <returns>No content on success.</returns>
        [HttpPut("updatePassword/{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePasswordAsync(Guid guid, [FromBody] UsuarioUpdateRequest updateRequest)
        {
            if (updateRequest is null)
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Payload inválido."
                    });

            updateRequest.Guid = guid;

            var data = await Task.FromResult(
                this._service.SaveData(
                    updateRequest: updateRequest));

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Usuário {guid} não encontrado.",
                    });

            return NoContent();
        }
    }
}