namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.Business.UniPayCheck;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;

    [Route("api/[controller]")]
    [ApiController]
    public class DemonstrativoPagamentoController : ControllerBase
    {


        public DemonstrativoPagamentoController()
        {

        }

        [HttpGet]
        public IActionResult GetDemonstrativosPagamento()
        {
            try
            {
                return Ok();

                //var agentes = this._repository.GetAll();

                //if (agentes is null)
                //{
                //    return NotFound(
                //        "Demonstrativos de Pagamento não encontrados!");
                //}

                //return Ok(
                //    agentes);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet("{guid}")]
        public IActionResult GetDemonstrativoPagamento(Guid guid)
        {
            return null;
            //try
            //{
            //    var record = this._repository.Get(guid);

            //    if (record is null)
            //    {
            //        return NotFound(
            //            $"Demonstrativo de Pagamento {guid} não encontrado!");
            //    }

            //    return Ok(
            //        record);
            //}
            //catch
            //{
            //    throw;
            //}
        }

        [HttpGet("{competencia}/matricula")]
        public IActionResult GetDemonstrativoPagamento(string competencia, string matricula)
        {
            try
            {
                var mdp = new MatriculaDemonstrativoPagamentoBusiness(
                    null);

                var dp = new MatriculaDemonstrativoPagamentoDto
                {
                    Competencia = "20230501",
                    Guid = Guid.NewGuid(),
                    GuidMatricula = Guid.NewGuid(),
                };

                //var dps = mdp.Get(
                //    competencia,
                //    matricula);

                var dps = new List<MatriculaDemonstrativoPagamentoDto>();
                dps.Add(dp);

                if (dps is null || dps.Count == 0)
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