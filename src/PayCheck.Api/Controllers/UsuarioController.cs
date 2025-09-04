namespace PayCheck.Api.Controllers
{
    using System;
    using System.Net;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
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
        /// <param name="loginDto"></param>
        /// <returns><see cref="IActionResult"/></returns>
        /// <response code="200">A busca foi realizada com sucesso.</response>
        /// <response code="404">A busca não encontrou resultados.</response>
        /// <response code="500">Ocorre</response>
        [Authorize]
        [HttpPost]
        public ApiResponseDto<UsuarioResponseDto> Authenticate([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                //var usuarioResponse = this._business.GetByUsername(
                //    loginDto.CpfEmailUsername).FirstOrDefault();

                var data = this._service.GetByUsername(
                    loginDto.CpfEmailUsername).FirstOrDefault();

                if (data is null)
                {
                    return new ApiResponseDto<UsuarioResponseDto>
                    {
                        Message = $"Usuário não encontrado para a credencial {loginDto.CpfEmailUsername}!",
                        StatusCode = HttpStatusCode.NotFound,
                    };
                }
                else
                {
                    data = this._service.CheckPasswordValid(
                        data.Guid,
                        loginDto.Password);

                    if (data != null)
                        return new ApiResponseDto<UsuarioResponseDto>
                        {
                            Data = data,
                            Success = true,
                            StatusCode = HttpStatusCode.OK,
                        };

                    return new ApiResponseDto<UsuarioResponseDto>
                    {
                        Message = $"A Senha não confere para o Usuário com a credencial {loginDto.CpfEmailUsername}!",
                        StatusCode = HttpStatusCode.NotFound,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<UsuarioResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("getNotificacoes/{guid}")]
        public ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>> GetNotificacoes(Guid guid)
        {
            try
            {
                var data = this._service.GetNotificacoes(
                    guidUsuario: guid);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>>
                {
                    Message = $"Notificações não encontradas para o Usuário {guid}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
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

                var usuarioResponse = this._service.SaveData(
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
                var usuarioResponse = this._service.SaveData(
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