namespace PayCheck.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Web;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using ARVTech.Shared.Email;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class AccessController : Controller
    {
        private readonly Uri _baseAddress = new(Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        private readonly IEmailService _emailService;

        public AccessController(IEmailService emailService)
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

            this._emailService = emailService;
        }

        public IActionResult ChangePassword()
        {
            try
            {
                if (Request.Query["parametro"].Count > 0 &&
                    Request.Query["parametro"][0] != null)
                {
                    //  Pega o parâmetro criptografado.
                    string query = Request.Query["parametro"][0].ToString();

                    // Descriptografa o parâmetro.
                    string key = QueryStringCryptography.Decrypt(
                        query,
                        Constants.EncryptionKey);

                    // Separa as variáveis utilizadas no parâmetro.
                    var parameters = HttpUtility.ParseQueryString(
                        key);

                    if (parameters != null && parameters.Count > 0)
                    {
                        //  Faz a validação dos parâmetros GuidUsuario e DataAtual.
                        if (parameters["GuidUsuario"] is null ||
                            !Guid.TryParse(
                                parameters["GuidUsuario"].ToString(),
                                out Guid guidUsuario) ||
                            parameters["DataAtual"] is null ||
                            !DateTime.TryParseExact(
                                parameters["DataAtual"].ToString(),
                                "ddMMyyyyHHmmss", 
                                CultureInfo.InvariantCulture, 
                                DateTimeStyles.AssumeLocal, 
                                out DateTime dataAtual))
                        {
                            throw new Exception("Não foi possível obter informações importantes do link.");
                        }

                        //  Pega a diferença da data de geração do link com a data atual.
                        TimeSpan diferencaDataLink = DateTime.Now.Subtract(dataAtual);

                        //  Se a diferença da data de geração do link com a data atual for superior a 120 minutos (2 Horas), mostra a mensagem.
                        //  Caso contrário, prossegue com o carregamento da tela para que o usuário possa atualizar a nova senha.
                        if (diferencaDataLink.Minutes > 120)
                        {
                            throw new Exception("Link expirado.");
                        }
                    }
                    else
                    {
                        throw new Exception("Link inválido.");
                    }
                }
                else
                {
                    throw new Exception("Link inválido.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(AlteracaoSenhaDto alteracaoSenhaDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            return RedirectToAction(
                "Index",
                "Home");
        }

        public IActionResult LinkActivation()
        {
            if (TempData.ContainsKey("GuidUsuario"))
            {
                return View();
            }

            ViewBag.ValidateMessage = "Usuário não encontrado para geração e envio do Link de Ativação.";

            return RedirectToAction(
                "Index",
                "Home");
        }

        [HttpPost]
        public IActionResult LinkActivation(ActivateDto activateDto)
        {
            try
            {
                if (TempData.ContainsKey("GuidUsuario"))
                {
                    Guid.TryParse(
                        TempData["GuidUsuario"].ToString(),
                        out Guid guidUsuario);

                    string dataAtualString = DateTime.Now.ToString("ddMMyyyyHHmmss");

                    TempData.Remove(
                        "GuidUsuario");

                    var parametros = $"GuidUsuario={guidUsuario:N}&DataAtual={dataAtualString}";

                    string key = QueryStringCryptography.Encrypt(
                        parametros,
                        Constants.EncryptionKey);

                    string url = $"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}";

                    string partialUrl = url.Substring(
                        0,
                        url.LastIndexOf("/"));

                    var linkActivation = $"{partialUrl}/ChangePassword?parametro={Uri.EscapeDataString(key)}";

                    this._emailService.SendMail(new EmailData
                    {
                        Body = $"Teste {linkActivation}",
                        ReceiverEmail = activateDto.Email,
                        ReceiverName = "Nome",
                        Subject = "Ativação de Conta"
                    });

                    ViewBag.SuccessMessage = $"E-mail enviado com sucesso para o endereço {activateDto.Email}.";
                }
                else
                {
                    ViewBag.ValidateMessage = "Usuário não encontrado para geração e envio do Link de Ativação.";
                }

                //return RedirectToAction(
                //    "Index",
                //    "Home");
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
                        if (TempData.ContainsKey("GuidUsuario"))
                        {
                            TempData.Remove("GuidUsuario");
                        }

                        TempData.Add(
                            "GuidUsuario",
                            usuarioResponse.Guid);

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