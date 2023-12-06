namespace PayCheck.Api.Controllers
{
    using System;
    using System.Net;
    using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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
        public ApiResponseDto<UsuarioResponseDto> Authenticate([FromBody] LoginRequestDto loginDto)
        {
            try
            {
                //var usuarioResponse = this._business.GetByUsername(
                //    loginDto.CpfEmailUsername).FirstOrDefault();

                var data = this._business.GetByUsername(
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
                    data = this._business.CheckPasswordValid(
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