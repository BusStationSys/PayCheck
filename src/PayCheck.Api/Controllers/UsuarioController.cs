namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Business.Interfaces;

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
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public IActionResult Authenticate([FromBody] LoginDto loginDto)
        {
            try
            {
                var usuarioAutenticado = this._business.GetByUsername(
                    loginDto.CpfEmailUsername);

                if (usuarioAutenticado is null)
                {
                    return NotFound(new
                    {
                        Message = $"Usuário não encontrado para a credencial {loginDto.CpfEmailUsername}!",
                        StatusCode = HttpStatusCode.NotFound,
                    });
                }
                else
                {
                    usuarioAutenticado = this._business.CheckPasswordValid(
                        usuarioAutenticado.Guid,
                        loginDto.Password);

                    if (usuarioAutenticado is null)
                    {
                        return NotFound(new
                        {
                            Message = $"A Senha não confere para o Usuário com a credencial {loginDto.CpfEmailUsername}!",
                            StatusCode = HttpStatusCode.NotFound,
                        });
                    }
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