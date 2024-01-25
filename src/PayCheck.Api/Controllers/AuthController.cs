namespace PayCheck.Api.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
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
        private readonly Authentication _authentication;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authentication"></param>
        public AuthController(IOptions<Authentication> authentication)
        {
            this._authentication = authentication.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] AuthRequestDto authDto)
        {
            if (authDto.Username != this._authentication.Username ||
                authDto.Password != this._authentication.Password)
                return BadRequest(
                    "Username ou Password inválidos!");

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
        private Task<string> GerarJwt(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(
                this._authentication.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = this._authentication.Audience,
                Expires = DateTime.UtcNow.AddMinutes(
                    this._authentication.ExpiresInMinutes),
                Issuer = this._authentication.Issuer,
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

            return Task.FromResult<string>(
                tokenHandler.WriteToken(
                    tokenHandler.CreateToken(
                        tokenDescriptor)));
        }
    }
}