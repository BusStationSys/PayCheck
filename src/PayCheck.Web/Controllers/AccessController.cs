namespace PayCheck.Web.Controllers
{
    using System;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
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

        public IActionResult LinkActivation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LinkActivation(ActivateDto activateDto)
        {
            try
            {
                if (TempData["GuidUsuario"] != null)
                {
                    string guidUsuario = ((Guid)TempData["GuidUsuario"]).ToString();
                    guidUsuario = guidUsuario.Replace(".", string.Empty);
                    guidUsuario = guidUsuario.Replace("-", string.Empty);

                    string dataAtual = DateTime.Now.ToString("yyyyMMddHHmmss");

                    var parametros = $"GUIDUsuario={guidUsuario}&DataAtual={dataAtual}";

                    string key = QueryStringCryptography.Encrypt(
                        parametros,
                        Constants.EncryptionKey);

                    string partialBaseURL = this._baseAddress.AbsoluteUri.Substring(0, this._baseAddress.AbsoluteUri.LastIndexOf("/"));

                    var linkActivation = $"{partialBaseURL}/Activate.aspx?parametro={Uri.EscapeDataString(
                        key)}";

                    TempData["GuidUsuario"] = null;

                    return RedirectToAction(
                        "Index",
                        "Home");
                }
                else
                {
                    ViewBag.ValidateMessage = "Usuário não encontrado para geração e envio do Link de Ativação.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
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
                var usuarioResponse = default(UsuarioResponse);

                string requestUri = @$"{this._httpClient.BaseAddress}/Usuario";

                using (var webApiHelper = new WebApiHelper(
                    requestUri,
                    this._tokenBearer))
                {
                    string stringJson = webApiHelper.ExecutePostAuthenticationByBearer(
                        loginDto);

                    usuarioResponse = JsonConvert.DeserializeObject<UsuarioResponse>(stringJson);
                }

                if (usuarioResponse?.Guid != Guid.Empty &&
                    usuarioResponse?.StatusCode == HttpStatusCode.OK)
                {
                    if (usuarioResponse.DataPrimeiroAcesso is null ||
                        !usuarioResponse.DataPrimeiroAcesso.HasValue)
                    {
                        TempData["GuidUsuario"] = usuarioResponse.Guid;

                        return RedirectToAction(
                            "LinkActivation",
                            "Access");
                    }

                    var claims = new List<Claim>()
                    {
                        new Claim(nameof(usuarioResponse.Guid), usuarioResponse.Guid.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, usuarioResponse.Guid.ToString()),
                        //new Claim(ClaimTypes.Name, usuarioResponse.Colaborador.Nome),
                        new Claim(ClaimTypes.Surname, usuarioResponse.Username),
                        //new Claim(ClaimTypes.Email, usuarioResponse.Colaborador.Pessoa.Email),
                        //new Claim(ClaimTypes.PostalCode, usuarioResponse.Colaborador.Pessoa.Cep),
                        //new Claim(ClaimTypes.StateOrProvince, usuarioResponse.Colaborador.Pessoa.Uf),
                        //new Claim("OtherProperty","OtherValue"),
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
                else
                {
                    if (usuarioResponse?.StatusCode == HttpStatusCode.NotFound)
                    {
                        ViewBag.ValidateMessage = usuarioResponse?.Message;
                    }
                    else if (usuarioResponse?.StatusCode == HttpStatusCode.BadRequest)
                    {
                        throw new Exception(usuarioResponse?.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }
    }
}
