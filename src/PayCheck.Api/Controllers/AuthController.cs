namespace PayCheck.Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettings"></param>
        public AuthController(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] AuthRequestDto authDto)
        {
            if (authDto.Username != this._appSettings.Username ||
                authDto.Password != this._appSettings.Password)
            {
                return BadRequest(
                    "Username ou Password inválidos!");
            }

            string jwtString = await this.GerarJwt(
                authDto.Username);

            return Ok(
                new AuthResponseDto
                {
                    Token = jwtString,
                    Username = authDto.Username,
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        private async Task<string> GerarJwt(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(
                this._appSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = this._appSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(
                    this._appSettings.ExpiresInMinutes),
                Issuer = this._appSettings.Issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        key),
                    SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(
                            ClaimTypes.Name,
                            username),  //User.Identity.Name
                    }),
            };

            return tokenHandler.WriteToken(
                tokenHandler.CreateToken(
                    tokenDescriptor));
        }
    }
}