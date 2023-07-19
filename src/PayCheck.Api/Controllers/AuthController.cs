namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppSettings _appSettings;

        //public AuthController(
        //    SignInManager<IdentityUser> signInManager,
        //    UserManager<IdentityUser> userManager,
        //    IOptions<AppSettings> appSettings)
        //{
        //    this._signInManager = signInManager;
        //    this._userManager = userManager;
        //    this._appSettings = appSettings.Value;
        //}
        public AuthController(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] AuthDto authDto)
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
                new AuthResponse
                {
                    Token = jwtString,
                    Username = authDto.Username,
                });
        }

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

        //private async Task<string> GerarJwt(string username)
        //{
        //    //var user = await this._userManager.FindByEmailAsync(email);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var key = Encoding.ASCII.GetBytes(
        //        this._appSettings.Secret);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Audience = this._appSettings.Audience,
        //        Expires = DateTime.UtcNow.AddMinutes(
        //            this._appSettings.ExpiresInMinutes),
        //        Issuer = this._appSettings.Issuer,
        //        SigningCredentials = new SigningCredentials(
        //            new SymmetricSecurityKey(
        //                key),
        //            SecurityAlgorithms.HmacSha256Signature),
        //    };

        //    return tokenHandler.WriteToken(
        //        tokenHandler.CreateToken(
        //            tokenDescriptor));
        //}
    }
}