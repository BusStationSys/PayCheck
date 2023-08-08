namespace PayCheck.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
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
        [Authorize]
        [HttpGet("{guid}")]
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
            catch
            {
                throw;
            }
        }
    }
}