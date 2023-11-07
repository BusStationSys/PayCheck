namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;
    using System.Net;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PessoaFisicaController : ControllerBase
    {
        private readonly IPessoaFisicaBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaFisicaController(IPessoaFisicaBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeletePessoaFisica(Guid guid)
        {
            try
            {
                var pf = this._business.Get(
                    guid);

                if (pf is null)
                {
                    return NotFound(
                        $"Pessoa Física {guid} não encontrada!");
                }

                this._business.Delete(
                    guid);

                return NoContent();
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
        /// <returns></returns>
        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoaFisica(Guid guid)
        {
            try
            {
                var pf = this._business.Get(
                    guid);

                if (pf is null)
                {
                    return NotFound(
                        $"Pessoa Física {guid} não encontrada!");
                }

                return Ok(
                    pf);
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
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPessoasFisicas()
        {
            try
            {
                var pfs = this._business.GetAll();

                if (pfs is null || pfs.Count() == 0)
                {
                    return NotFound(
                        $"Pessoas Físicas não encontradas!");
                }

                return Ok(
                    pfs);
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
        /// <param name="createDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertPessoaFisica([FromBody] PessoaFisicaRequestCreateDto createDto)
        {
            try
            {
                var pessoaFisicaResponse = this._business.SaveData(
                    updateDto: updateDto);

                matriculaEspelhoPontoResponse.StatusCode = HttpStatusCode.Created;

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
        //[HttpPut("{guid}")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult UpdateMatriculaEspelhoPonto(Guid guid, [FromBody] PessoaFisicaRequestDto updateDto)
        //{
        //    try
        //    {
        //        updateDto.Guid = guid;

        //        var matriculaEspelhoPontoResponse = this._business.SaveData(
        //            updateDto: updateDto);

        //        matriculaEspelhoPontoResponse.StatusCode = HttpStatusCode.NoContent;

        //        return Ok(
        //            matriculaEspelhoPontoResponse);

        //        //var matriculaEspelhoPontoResponse = this._business.Get(
        //        //    guid);

        //        //if (matriculaEspelhoPontoResponse is null)
        //        //{
        //        //    return StatusCode(
        //        //        StatusCodes.Status404NotFound,
        //        //        $"Espelho de Ponto {guid} não encontrado!");
        //        //}
        //        //else if(matriculaEspelhoPontoResponse.DataConfirmacao != null)
        //        //{
        //        //    return StatusCode(
        //        //        StatusCodes.Status404NotFound,
        //        //        $"Espelho Ponto {guid} confirmado em {matriculaEspelhoPontoResponse.DataConfirmacao.Value.LocalDateTime:dd/MM/yyyy HH:mm:ss}.");
        //        //}

        //        //var matriculaEspelhoPontoRequestUpdateDto = this._mapper.Map<MatriculaEspelhoPontoRequestUpdateDto>(
        //        //    matriculaEspelhoPontoResponse);

        //        //matriculaEspelhoPontoResponse = this._business.SaveData(
        //        //    updateDto: matriculaEspelhoPontoRequestUpdateDto);

        //        //matriculaEspelhoPontoResponse.StatusCode = HttpStatusCode.NoContent;

        //        //return StatusCode(
        //        //    StatusCodes.Status204NoContent,
        //        //    usuarioResponse);

        //        //return Ok(
        //        //    matriculaEspelhoPontoResponse);

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