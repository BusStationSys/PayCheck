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
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UsuarioController(IUsuarioService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AuthenticateAsync([FromBody] LoginRequest loginRequest)
        {
            var data = this._service.GetByUsername(
                loginRequest.CpfEmailUsername).FirstOrDefault();

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Usuário não encontrado para a credencial {loginRequest.CpfEmailUsername}!",
                    });

            data = this._service.CheckPasswordValid(
                data.Guid,
                loginRequest.Password);

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"A Senha não confere para o Usuário com a credencial {loginRequest.CpfEmailUsername}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("Notificacoes/{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNotificacoesAsync(Guid guid)
        {
            var data = this._service.GetNotificacoes(
                guidUsuario: guid);

            if (data != null &&
                data.Count() > 0)
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
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [Authorize]
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

            //return Ok(
            //    data);

            return StatusCode(
                StatusCodes.Status204NoContent,
                data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="updateRequest"></param>
        /// <returns></returns>
        [Authorize]
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

            return StatusCode(
                StatusCodes.Status204NoContent,
                data);

            //return Ok(
            //    data);

            //return CreatedAtAction(
            //    nameof(
            //        this.GetAgente),
            //    new
            //    {
            //        id = agenteResponseDto.CodigoAgente,
            //    },
            //    agenteResponseDto);

            //return NoContent();
        }
    }
}