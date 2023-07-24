namespace PayCheck.Web.Controllers
{
    using System.Security.Claims;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class AccessController : Controller
    {
        private readonly Uri _baseAddress = new(Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        public AccessController()
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = this._baseAddress,
            };

            using (var webApiHelper = new WebApiHelper(
                string.Concat(
                    this._baseAddress,
                    "/auth"),
                "arvtech",
                "(@rV73Ch)"))
            {
                var authDto = new AuthDto
                {
                    Username = "arvtech",
                    Password = "(@rV73Ch)",
                };

                string stringJson = webApiHelper.ExecutePostAuthenticationByBasic(
                    authDto);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                    stringJson);

                this._tokenBearer = authResponse.Token;
            }
        }

        public IActionResult Login()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            if (claimsPrincipal.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/Usuario";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecutePostAuthenticationByBearer(
                    loginDto);

            }

            if ((loginDto.CpfEmailUsername == "andre" || loginDto.CpfEmailUsername == "arvtech@arvtech.com.br") &&
                (loginDto.Password == "123456"))
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,loginDto.CpfEmailUsername),
                    new Claim("OtherProperty","OtherValue"),
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                AuthenticationProperties authenticationProperties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = loginDto.KeepLoggedIn,
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        claimsIdentity),
                    authenticationProperties);

                return RedirectToAction(
                    "Index",
                    "Home");
            }

            ViewData["ValidateMessage"] = "Usuário não encontrado.";

            return View();
        }
    }
}
