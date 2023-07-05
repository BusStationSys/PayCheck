namespace PayCheck.Api.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

    [Route("api/[controller]")]
    [ApiController]
    public class DemonstrativoPagamentoController : ControllerBase
    {
        private readonly IMatriculaDemonstrativoPagamentoBusiness _business;

        public DemonstrativoPagamentoController(IMatriculaDemonstrativoPagamentoBusiness business)
        {
            // this._repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        [HttpGet]
        public IActionResult GetDemonstrativosPagamento()
        {
            try
            {
                var dps = this._business.GetAll();

                if (dps is null || dps.Count() == 0)
                {
                    return NotFound(
                        $"Demonstrativos de Pagamento não encontrado!");
                }

                return Ok(
                    dps);
            }
            catch
            {
                throw;
            }
        }

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
    }
}