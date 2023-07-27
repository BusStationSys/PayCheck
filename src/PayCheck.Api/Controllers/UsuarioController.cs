namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;
    using System.Diagnostics;
    using System.Net;

    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioBusiness _business;

        public UsuarioController(IUsuarioBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult Authenticate([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuarioAutenticado = this._business.Authenticate(
                    loginDto.CpfEmailUsername,
                    loginDto.Password);

                if (usuarioAutenticado is null)
                {
                    return NotFound(new
                    {
                        Message = $"Usuário não encontrado para as credenciais {loginDto.CpfEmailUsername}!",
                        StatusCode = HttpStatusCode.NotFound,
                    });
                }

                return Ok(new
                {
                    usuarioAutenticado,
                    StatusCode = HttpStatusCode.OK,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.BadRequest,
                });
            }
        }
    }
}