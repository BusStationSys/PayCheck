namespace PayCheck.Api.Controllers
{
    using System;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using System.Net;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DemonstrativoPagamentoController : ControllerBase
    {
        private readonly IMatriculaDemonstrativoPagamentoBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DemonstrativoPagamentoController(IMatriculaDemonstrativoPagamentoBusiness business)
        {
            // this._repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDemonstrativosPagamento()
        {
            try
            {
                var dps = this._business.GetAll();

                if (dps is null || dps.Count() == 0)
                {
                    return NotFound(
                        $"Demonstrativos de Pagamento não encontrados!");
                }

                return Ok(
                    dps);
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
        //[Authorize]
        [HttpGet("{guid}")]
        public IActionResult GetDemonstrativoPagamento(Guid guid)
        {
            try
            {
                var dp = this._business.Get(
                    guid);

                if (dp is null)
                {
                    return NotFound(
                        $"Demonstrativo de Pagamento {guid} não encontrado!");
                }

                return Ok(
                    dp);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guidColaborador"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("getDemonstrativoPagamentoByGuidColaborador/{guidColaborador}")]
        public IActionResult GetDemonstrativoPagamentoByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                var dp = this._business.GetByGuidColaborador(
                    guidColaborador);

                if (dp is null)
                {
                    return NotFound(
                        $"Demonstrativo de Pagamento {guidColaborador} não encontrado!");
                }

                return Ok(
                    dp);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{competencia}/{matricula}")]
        public IActionResult GetDemonstrativoPagamento(string competencia, string matricula)
        {
            try
            {
                var dps = this._business.Get(
                    competencia,
                    matricula);

                if (dps is null)
                {
                    return NotFound(
                        "Demonstrativos de Pagamento não encontrados!");
                }

                return Ok(
                    dps);
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
        //[Authorize]
        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateMatriculaDemonstrativoPagamento(Guid guid, [FromBody] MatriculaDemonstrativoPagamentoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var matriculaDemonstrativoPagamentoResponse = this._business.SaveData(
                    updateDto: updateDto);

                matriculaDemonstrativoPagamentoResponse.StatusCode = HttpStatusCode.NoContent;

                return Ok(
                    matriculaDemonstrativoPagamentoResponse);
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