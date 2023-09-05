namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UsuarioController(IUsuarioBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns><see cref="IActionResult"/></returns>
        /// <response code="200">A busca foi realizada com sucesso.</response>
        /// <response code="404">A busca não encontrou resultados.</response>
        /// <response code="500">Ocorre</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Authenticate([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuarioResponse = this._business.GetByUsername(
                    loginDto.CpfEmailUsername);

                if (usuarioResponse is null)
                {
                    return NotFound(new
                    {
                        Message = $"Usuário não encontrado para a credencial {loginDto.CpfEmailUsername}!",
                        StatusCode = HttpStatusCode.NotFound,
                    });
                }
                else
                {
                    usuarioResponse = this._business.CheckPasswordValid(
                        usuarioResponse.Guid,
                        loginDto.Password);

                    if (usuarioResponse is null)
                    {
                        return NotFound(new
                        {
                            Message = $"A Senha não confere para o Usuário com a credencial {loginDto.CpfEmailUsername}!",
                            StatusCode = HttpStatusCode.NotFound,
                        });
                    }
                    else
                    {
                        usuarioResponse.StatusCode = HttpStatusCode.OK;
                    }
                }

                return Ok(
                    usuarioResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateUsuario(Guid guid, [FromBody] UsuarioRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var usuarioResponse = this._business.SaveData(
                    updateDto: updateDto);

                usuarioResponse.StatusCode = HttpStatusCode.NoContent;

                //return StatusCode(
                //    StatusCodes.Status204NoContent,
                //    usuarioResponse);

                return Ok(
                    usuarioResponse);

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
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("updatePassword/{guid}")]
        //[HttpGet("getGruposByNumeroRemessa/{numeroRemessa}")]
        //[HttpGet("getByNumeroRemessa/{numeroRemessa}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdatePassword(Guid guid, [FromBody] UsuarioRequestUpdateDto updateDto)
        {
            try
            {
                var usuarioResponse = this._business.SaveData(
                    updateDto: updateDto);

                usuarioResponse.StatusCode = HttpStatusCode.NoContent;

                //return StatusCode(
                //    StatusCodes.Status204NoContent,
                //    usuarioResponse);

                return Ok(
                    usuarioResponse);

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
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
    }
}