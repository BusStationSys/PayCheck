namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EspelhoPontoController : ControllerBase
    {
        private readonly IMatriculaEspelhoPontoBusiness _business;

        //private readonly Mapper _mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EspelhoPontoController(IMatriculaEspelhoPontoBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));

            //var mapperConfiguration = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
            //    cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
            //});

            //this._mapper = new Mapper(
            //    mapperConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult GetEspelhosPonto()
        {
            try
            {
                var eps = this._business.GetAll();

                if (eps is null || eps.Count() == 0)
                {
                    return NotFound(
                        $"Espelhos de Ponto não encontrados!");
                }

                return Ok(
                    eps);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{guid}")]
        public IActionResult GetEspelhoPonto(Guid guid)
        {
            try
            {
                var matriculaEspelhoPontoResponse = this._business.Get(
                    guid);

                if (matriculaEspelhoPontoResponse is null)
                {
                    return NotFound(
                        $"Espelho de Ponto {guid} não encontrado!");
                }

                return Ok(
                    matriculaEspelhoPontoResponse);
            }
            catch
            {
                throw;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMatriculaEspelhoPonto(Guid guid, [FromBody] MatriculaEspelhoPontoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var matriculaEspelhoPontoResponse = this._business.SaveData(
                    updateDto: updateDto);

                matriculaEspelhoPontoResponse.StatusCode = HttpStatusCode.NoContent;

                return Ok(
                    matriculaEspelhoPontoResponse);

                //var matriculaEspelhoPontoResponse = this._business.Get(
                //    guid);

                //if (matriculaEspelhoPontoResponse is null)
                //{
                //    return StatusCode(
                //        StatusCodes.Status404NotFound,
                //        $"Espelho de Ponto {guid} não encontrado!");
                //}
                //else if(matriculaEspelhoPontoResponse.DataConfirmacao != null)
                //{
                //    return StatusCode(
                //        StatusCodes.Status404NotFound,
                //        $"Espelho Ponto {guid} confirmado em {matriculaEspelhoPontoResponse.DataConfirmacao.Value.LocalDateTime:dd/MM/yyyy HH:mm:ss}.");
                //}

                //var matriculaEspelhoPontoRequestUpdateDto = this._mapper.Map<MatriculaEspelhoPontoRequestUpdateDto>(
                //    matriculaEspelhoPontoResponse);

                //matriculaEspelhoPontoResponse = this._business.SaveData(
                //    updateDto: matriculaEspelhoPontoRequestUpdateDto);

                //matriculaEspelhoPontoResponse.StatusCode = HttpStatusCode.NoContent;

                //return StatusCode(
                //    StatusCodes.Status204NoContent,
                //    usuarioResponse);

                //return Ok(
                //    matriculaEspelhoPontoResponse);

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="guid"></param>
        ///// <param name="updateDto"></param>
        ///// <returns></returns>
        //[Authorize]
        //[HttpPut("updatePassword/{guid}")]
        ////[HttpGet("getGruposByNumeroRemessa/{numeroRemessa}")]
        ////[HttpGet("getByNumeroRemessa/{numeroRemessa}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        ////[ProducesResponseType(StatusCodes.Status200OK)]
        ////[ProducesResponseType(StatusCodes.Status400BadRequest)]
        ////[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        ////[ProducesResponseType(StatusCodes.Status404NotFound)]

        //public IActionResult UpdatePassword(Guid guid, [FromBody] UsuarioRequestUpdateDto updateDto)
        //{
        //    try
        //    {
        //        var usuarioResponse = this._business.SaveData(
        //            updateDto: updateDto);

        //        usuarioResponse.StatusCode = HttpStatusCode.NoContent;

        //        //return StatusCode(
        //        //    StatusCodes.Status204NoContent,
        //        //    usuarioResponse);

        //        return Ok(
        //            usuarioResponse);

        //        //return CreatedAtAction(
        //        //    nameof(
        //        //        this.GetAgente),
        //        //    new
        //        //    {
        //        //        id = agenteResponseDto.CodigoAgente,
        //        //    },
        //        //    agenteResponseDto);

        //        //return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(
        //            StatusCodes.Status500InternalServerError,
        //            ex.Message);
        //    }
        //}
    }
}