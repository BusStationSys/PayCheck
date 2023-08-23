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
    public class EspelhoPontoController : ControllerBase
    {
        private readonly IMatriculaEspelhoPontoBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EspelhoPontoController(IMatriculaEspelhoPontoBusiness business)
        {
            // this._repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this._business = business ?? throw new ArgumentNullException(nameof(business));
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
    }
}