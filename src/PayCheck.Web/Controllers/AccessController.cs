namespace PayCheck.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using ARVTech.Shared;
    using ARVTech.Shared.Email;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PayCheck.Web.Common;
    using PayCheck.Web.Extensions;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Models;
    using PayCheck.Web.Services.Interfaces;

    public class AccessController : Controller
    {
        //  private readonly string _tokenBearer;

        private readonly IEmailService _emailService;

        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        /// <summary>
        /// /// Initializes a new instance of the <see cref="AccessController"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <exception cref="Exception"></exception>
        public AccessController(IEmailService emailService, IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._emailService = emailService;

            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
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
                        //  Faz a validação dos parâmetros GuidUsuario, DataAtual e Email.
                        if (parameters["GuidUsuario"] is null ||
                            !Guid.TryParse(
                                parameters["GuidUsuario"].ToString(),
                                out Guid guidUsuario) ||
                            parameters["DataAtual"] is null ||
                            !DateTimeOffset.TryParseExact(
                                parameters["DataAtual"].ToString(),
                                "ddMMyyyyHHmmss",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out DateTimeOffset dataAtual) ||
                            parameters["Email"] is null ||
                            string.IsNullOrEmpty(
                                parameters["Email"]) ||
                            parameters["GuidColaborador"] is null ||
                            !Guid.TryParse(
                                parameters["GuidColaborador"].ToString(),
                                out Guid guidColaborador) ||
                            parameters["Username"] is null ||
                            string.IsNullOrEmpty(
                                parameters["Username"]))
                        {
                            throw new Exception("Não foi possível obter informações importantes do link.");
                        }

                        //  Pega a diferença da data de geração do link com a data atual.
                        DateTime dataDiferenca = DateTime.Now;

                        TimeSpan diferencaDataLink = dataDiferenca.Subtract(dataAtual.LocalDateTime);

                        //  Se a diferença da data de geração do link com a data atual for superior a 120 minutos (2 Horas), mostra a mensagem.
                        //  Caso contrário, prossegue com o carregamento da tela para que o usuário possa atualizar a nova senha.
                        if (diferencaDataLink.TotalMinutes > 120)
                        {
                            throw new Exception("Link expirado.");
                        }

                        TempData.Remove(
                            "ChangePasswordGuidUsuario");

                        TempData.Remove(
                            "ChangePasswordEmail");

                        TempData.Remove(
                            "ChangePasswordGuidColaborador");

                        TempData.Remove(
                            "ChangePasswordUsername");

                        TempData.Remove(
                            "ChangePasswordIdPerfilUsuario");

                        TempData["ChangePasswordGuidUsuario"] = guidUsuario;
                        TempData["ChangePasswordEmail"] = parameters["Email"];
                        TempData["ChangePasswordGuidColaborador"] = guidColaborador;
                        TempData["ChangePasswordUsername"] = parameters["Username"];
                        TempData["ChangePasswordIdPerfilUsuario"] = parameters["IdPerfilUsuario"];
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
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            if (!ModelState.IsValid)
                return View();

            var usuarioUpdateRequest = new UsuarioUpdateRequest
            {
                Guid = Guid.Parse(TempData["ChangePasswordGuidUsuario"].ToString()),
                GuidColaborador = Guid.Parse(TempData["ChangePasswordGuidColaborador"].ToString()),
                Email = TempData["ChangePasswordEmail"].ToString(),
                DataPrimeiroAcesso = DateTimeOffset.UtcNow,
                Password = changePasswordRequest.Password,
                ConfirmPassword = changePasswordRequest.ConfirmPassword,
                Username = TempData["ChangePasswordUsername"].ToString(),
                IdPerfilUsuario = int.Parse(TempData["ChangePasswordIdPerfilUsuario"].ToString()),
            };

            string requestUri = @$"Usuario/{usuarioUpdateRequest.Guid:N}";

            string requestBody = JsonConvert.SerializeObject(
                usuarioUpdateRequest,
                Formatting.Indented);

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Put,
                requestUri,
                requestBody))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    string message = "Erro desconhecido ao alterar a senha.";

                    if (responseBody.IsValidJson())
                    {
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(
                            responseBody);

                        message = problemDetails?.Detail ??
                            problemDetails?.Title ??
                            "Erro ao alterar a senha.";
                    }

                    TempData.AddNotification(
                        NotificationType.Danger,
                        message);

                    return View();
                }

                var usuarioResponse = responseBody.IsValidJson() ?
                    JsonConvert.DeserializeObject<UsuarioResponse>(responseBody) :
                    null;

                TempData.AddNotification(
                    NotificationType.Success,
                    $"Senha alterada para o usuário <strong>{usuarioResponse?.Username}</strong>.");

                return RedirectToAction(
                    "Index",
                    "Home");
            }
        }

        public IActionResult LinkActivation()
        {
            ViewBag.NomeColaborador = null;

            if (TempData.ContainsKey("GuidUsuario"))
            {
                string emailColaborador = string.Empty;

                if (TempData.ContainsKey("NomeColaborador"))
                    ViewBag.NomeColaborador = TempData.Peek("NomeColaborador");

                if (TempData.ContainsKey("EmailColaborador"))
                    emailColaborador = TempData.Peek("EmailColaborador").ToString();

                TempData.Keep();

                return View(
                    new ActivateViewModel()
                    {
                        Email = emailColaborador,
                    });
            }

            ViewBag.ValidateMessage = "Usuário não encontrado para geração e envio do Link de Ativação.";

            //return RedirectToAction(
            //    "Index",
            //    "Home");

            return View(
                "Login");
        }

        [HttpPost]
        public IActionResult LinkActivation(ActivateViewModel activateViewModel)
        {
            try
            {
                ViewBag.ErrorMessage = null;
                ViewBag.SuccessMessage = null;
                ViewBag.ValidateMessage = null;

                if (TempData.ContainsKey("GuidUsuario"))
                {
                    Guid.TryParse(
                        TempData["GuidUsuario"].ToString(),
                        out Guid guidUsuario);

                    Guid.TryParse(
                        TempData["GuidColaborador"].ToString(),
                        out Guid guidColaborador);

                    int.TryParse(
                        TempData["IdPerfilUsuario"].ToString(),
                        out int idPerfilUsuario);

                    string dataAtualString = DateTime.Now.ToString("ddMMyyyyHHmmss");

                    string nomeColaborador = TempData["NomeColaborador"].ToString();

                    string username = TempData["Username"].ToString();

                    TempData.Remove(
                        "GuidUsuario");

                    TempData.Remove(
                        "GuidColaborador");

                    TempData.Remove(
                        "EmailColaborador");

                    TempData.Remove(
                        "NomeColaborador");

                    TempData.Remove(
                        "Username");

                    TempData.Remove(
                        "IdPerfilUsuario");

                    var parametros = $"GuidUsuario={guidUsuario:N}&DataAtual={dataAtualString}&Email={activateViewModel.Email}&GuidColaborador={guidColaborador:N}&Username={username}&IdPerfilUsuario={idPerfilUsuario}";

                    string key = QueryStringCryptography.Encrypt(
                        parametros,
                        Constants.EncryptionKey);

                    string url = $"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}";

                    string partialUrl = url.Substring(
                        0,
                        url.LastIndexOf('/'));

                    var linkActivation = $"{partialUrl}/ChangePassword?parametro={Uri.EscapeDataString(key)}";

                    string bodyString = $@"Olá {nomeColaborador},

Para começar a utilizar os recursos incríveis que são oferecidos, é necessário ativar sua conta.

Clique no link abaixo para ativar sua conta agora mesmo:
{linkActivation}

Se por algum motivo o link acima não funcionar, você pode copiar e colar o endereço completo em seu navegador:
{linkActivation}

É importante lembrar que este link é válido por 2 horas. Portanto, é sugerido que ele seja acessado o mais breve possível.

Em caso de dúvidas ou problemas durante o processo de ativação, por gentileza, não hesite em entrar em contato com a nossa equipe de suporte, que terá muita satisfação em ajudá-lo.

É uma honra ter você conosco e agradecemos por se juntar a nós. Estamos ansiosos para fornecer a melhor experiência possível.

Atenciosamente,

A Equipe de Suporte PayCheck®.";

                    this._emailService.SendMail(new EmailData
                    {
                        Body = bodyString,
                        ReceiverEmail = activateViewModel.Email,
                        ReceiverName = nomeColaborador,
                        Subject = "Ativação de Conta"
                    });

                    ViewBag.SuccessMessage = $"E-mail enviado para o endereço {activateViewModel.Email}.";
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

            // return View("Login");

            return View(
                new ActivateViewModel()
                {
                    Email = activateViewModel.Email,
                });
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
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            ViewBag.ErrorMessage = null;
            ViewBag.SuccessMessage = null;
            ViewBag.ValidateMessage = null;

            if (!ModelState.IsValid)
            {
                var errorMessageHtml = new StringBuilder();

                var modelStateErrors = this.ModelState.Keys.OrderBy(x => x).SelectMany(key => this.ModelState[key].Errors);

                if (modelStateErrors != null &&
                    modelStateErrors.Any())
                {
                    errorMessageHtml.Append("<p></p>");

                    foreach (var modelStateError in modelStateErrors)
                    {
                        errorMessageHtml.Append("<p style=\"text-align:justify\">");

                        errorMessageHtml.Append($"- {modelStateError.ErrorMessage}");

                        errorMessageHtml.Append("</p>");
                    }
                }

                ViewBag.ValidateMessage = errorMessageHtml.ToString();

                return View(
                    vm);
            }

            var loginRequest = this._mapper.Map<LoginRequest>(
                vm);

            string requestUri = "Usuario";

            string fromBodyString = JsonConvert.SerializeObject(
                loginRequest,
                Formatting.Indented);

            var usuarioResponse = default(
                UsuarioResponse);

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Post,
                requestUri,
                fromBodyString))
            {
                var responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                    {
                        usuarioResponse = JsonConvert.DeserializeObject<UsuarioResponse>(
                            responseBody);

                        if (usuarioResponse.DataPrimeiroAcesso is null ||
                            !usuarioResponse.DataPrimeiroAcesso.HasValue)
                        {
                            TempData.Remove("GuidUsuario");
                            TempData.Remove("GuidColaborador");
                            TempData.Remove("NomeColaborador");
                            TempData.Remove("EmailColaborador");
                            TempData.Remove("Username");
                            TempData.Remove("IdPerfilUsuario");

                            TempData["GuidUsuario"] = usuarioResponse.Guid;
                            TempData["GuidColaborador"] = usuarioResponse.Colaborador.Guid;
                            TempData["NomeColaborador"] = usuarioResponse.Colaborador.Nome;
                            TempData["EmailColaborador"] = usuarioResponse.Colaborador.Pessoa.Email;
                            TempData["Username"] = usuarioResponse.Username;
                            TempData["IdPerfilUsuario"] = usuarioResponse.IdPerfilUsuario;

                            return RedirectToAction(
                                "LinkActivation",
                                "Access");
                        }
                        else
                        {
                            string emailUsuario = string.Empty;
                            string guidColaborador = string.Empty;
                            string nomeColaborador = string.Empty;

                            if (usuarioResponse.GuidColaborador != null &&
                                usuarioResponse.GuidColaborador.HasValue &&
                                usuarioResponse.GuidColaborador.Value != Guid.Empty)
                            {
                                emailUsuario = usuarioResponse.Email;
                                guidColaborador = usuarioResponse.Colaborador.Guid.ToString();
                                nomeColaborador = usuarioResponse.Colaborador.Nome;
                            }
                            else
                                nomeColaborador = usuarioResponse.Username;

                            var claims = new List<Claim>
                            {
                                new (ClaimTypes.NameIdentifier, usuarioResponse.Guid.ToString()),
                                new (ClaimTypes.Name, nomeColaborador),
                                new (ClaimTypes.Email, emailUsuario),

                                new ($"{nameof(
                                    UsuarioResponse.Guid)}Usuario",
                                    usuarioResponse.Guid.ToString()),

                                new ($"{nameof(
                                    UsuarioResponse.Colaborador.Guid)}Colaborador",
                                    guidColaborador),

                                new ($"{nameof(
                                    UsuarioResponse.Colaborador.Nome)}Colaborador",
                                    nomeColaborador),

                                new (nameof(
                                    UsuarioResponse.Username),
                                    usuarioResponse.Username),

                                new ($"{nameof(
                                    UsuarioResponse.Email)}Usuario",
                                    emailUsuario),

                                //new Claim("OtherProperty","OtherValue"),

                                new ($"{nameof(
                                    UsuarioResponse.IdPerfilUsuario)}",
                                    usuarioResponse.IdPerfilUsuario.ToString()),
                            };

                            ClaimsIdentity claimsIdentity = new(
                                claims,
                                CookieAuthenticationDefaults.AuthenticationScheme);

                            AuthenticationProperties authenticationProperties = new()
                            {
                                AllowRefresh = true,
                                IsPersistent = loginRequest.KeepLoggedIn,
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
                    }
                }
                else
                {
                    string message = string.Empty;

                    if (responseBody.IsValidJson())
                    {
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(
                            responseBody);

                        //ViewBag.ErrorMessage = problemDetails?.Detail ??
                        //    problemDetails?.Title ??
                        //    "<p>Erro ao efetuar Login.</p>";

                        message = problemDetails?.Detail ??
                            problemDetails?.Title ??
                            "Erro ao efetuar Login.";

                        message = $"<p>{message}</p>";
                    }
                    else
                    {
                        //  ViewBag.ErrorMessage = "<p>Erro desconhecido ao efetuar Login.</p>";

                        message = "<p>Erro desconhecido ao efetuar Login.</p>";
                    }

                    TempData.AddNotification(
                        NotificationType.Danger,
                        message);
                }
            }

            vm.Password = string.Empty;

            return View(
                vm);
        }
    }
}