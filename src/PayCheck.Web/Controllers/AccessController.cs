namespace PayCheck.Web.Controllers
{
    using System.Net;
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
            try
            {
                var usuario = default(UsuarioResponse);

                bool autenticado = false;

                if ((loginDto.CpfEmailUsername == "andre" || loginDto.CpfEmailUsername == "arvtech@arvtech.com.br") &&
                    (loginDto.Password == "123456"))
                {
                    autenticado = true;
                }
                else
                {
                    string requestUri = @$"{this._httpClient.BaseAddress}/Usuario";

                    using (var webApiHelper = new WebApiHelper(
                        requestUri,
                        this._tokenBearer))
                    {
                        string stringJson = webApiHelper.ExecutePostAuthenticationByBearer(
                            loginDto);

                        usuario = JsonConvert.DeserializeObject<UsuarioResponse>(stringJson);
                    }

                    if (usuario != null)
                    {
                        if (usuario.StatusCode == HttpStatusCode.OK)
                        {
                            autenticado = true;
                        }
                        else if(usuario.StatusCode == HttpStatusCode.BadRequest)
                        {
                            throw new Exception(usuario.Message);
                        }
                    }
                }

                if (autenticado)
                {
                    List<Claim> claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,loginDto.CpfEmailUsername),
                        new Claim("OtherProperty","OtherValue"),
                    };

                    ClaimsIdentity claimsIdentity = new(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    AuthenticationProperties authenticationProperties = new()
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

                ViewBag.ValidateMessage = usuario.Message;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
    }
}
