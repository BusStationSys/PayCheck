﻿namespace PayCheck.Web.Controllers
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Security.Claims;
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
                        TimeSpan diferencaDataLink = DateTime.Now.Subtract(dataAtual.LocalDateTime);

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

                        TempData["ChangePasswordGuidUsuario"] = guidUsuario;
                        TempData["ChangePasswordEmail"] = parameters["Email"];
                        TempData["ChangePasswordGuidColaborador"] = guidColaborador;
                        TempData["ChangePasswordUsername"] = parameters["Username"];
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
            try
            {
                ViewBag.ErrorMessage = null;
                ViewBag.SuccessMessage = null;
                ViewBag.ValidateMessage = null;

                if (!ModelState.IsValid)
                {
                    return View();
                }

                var usuarioUpdateDto = new UsuarioRequestUpdateDto
                {
                    Guid = Guid.Parse(TempData["ChangePasswordGuidUsuario"].ToString()),
                    GuidColaborador = Guid.Parse(TempData["ChangePasswordGuidColaborador"].ToString()),
                    Email = TempData["ChangePasswordEmail"].ToString(),
                    DataPrimeiroAcesso = DateTimeOffset.UtcNow,
                    Password = alteracaoSenhaDto.Password,
                    ConfirmPassword = alteracaoSenhaDto.ConfirmPassword,
                    Username = TempData["ChangePasswordUsername"].ToString(),
                };

                var usuarioResponse = default(UsuarioResponse);

                string requestUri = @$"{this._httpClient.BaseAddress}/Usuario/{usuarioUpdateDto.Guid:N}";

                using (var webApiHelper = new WebApiHelper(
                    requestUri,
                    this._tokenBearer))
                {
                    string stringJson = webApiHelper.ExecutePutAuthenticationByBearer(
                        usuarioUpdateDto);

                    usuarioResponse = JsonConvert.DeserializeObject<UsuarioResponse>(stringJson);
                }

                ViewBag.SuccessMessage = $"Senha alterada para o usuário {usuarioResponse.Username}.";

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

        public IActionResult LinkActivation()
        {
            ViewBag.NomeColaborador = null;

            if (TempData.ContainsKey("GuidUsuario"))
            {
                if (TempData.ContainsKey("NomeColaborador"))
                {
                    ViewBag.NomeColaborador = TempData.Peek("NomeColaborador");
                }

                TempData.Keep();

                return View();
            }

            ViewBag.ValidateMessage = "Usuário não encontrado para geração e envio do Link de Ativação.";

            //return RedirectToAction(
            //    "Index",
            //    "Home");

            return View("Login");
        }

        [HttpPost]
        public IActionResult LinkActivation(ActivateDto activateDto)
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

                    string dataAtualString = DateTime.Now.ToString("ddMMyyyyHHmmss");

                    string nomeColaborador = TempData["NomeColaborador"].ToString();

                    string username = TempData["Username"].ToString();

                    TempData.Remove(
                        "GuidUsuario");

                    TempData.Remove(
                        "GuidColaborador");

                    TempData.Remove(
                        "NomeColaborador");

                    TempData.Remove(
                        "Username");

                    var parametros = $"GuidUsuario={guidUsuario:N}&DataAtual={dataAtualString}&Email={activateDto.Email}&GuidColaborador={guidColaborador:N}&Username={username}";

                    string key = QueryStringCryptography.Encrypt(
                        parametros,
                        Constants.EncryptionKey);

                    string url = $"{Request.Scheme}://{Request.Host.Value}{Request.Path.Value}";

                    string partialUrl = url.Substring(
                        0,
                        url.LastIndexOf("/"));

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
                        ReceiverEmail = activateDto.Email,
                        ReceiverName = nomeColaborador,
                        Subject = "Ativação de Conta"
                    });

                    ViewBag.SuccessMessage = $"E-mail enviado para o endereço {activateDto.Email}.";
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
                        TempData.Remove("GuidUsuario");
                        TempData.Remove("GuidColaborador");
                        TempData.Remove("NomeColaborador");
                        TempData.Remove("Username");

                        TempData["GuidUsuario"] = usuarioResponse.Guid;
                        TempData["GuidColaborador"] = usuarioResponse.Colaborador.Guid;
                        TempData["NomeColaborador"] = usuarioResponse.Colaborador.Nome;
                        TempData["Username"] = usuarioResponse.Username;

                        return RedirectToAction(
                            "LinkActivation",
                            "Access");
                    }
                    else
                    {
                        string guidColaborador = string.Empty;
                        string nomeColaborador = string.Empty;

                        if (usuarioResponse.GuidColaborador != null &&
                            usuarioResponse.GuidColaborador.HasValue &&
                            usuarioResponse.GuidColaborador.Value != Guid.Empty)
                        {
                            guidColaborador = usuarioResponse.Colaborador.Guid.ToString();
                            nomeColaborador = usuarioResponse.Colaborador.Nome;
                        }

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, usuarioResponse.Guid.ToString()),
                            new Claim(ClaimTypes.Name, usuarioResponse.Colaborador.Nome),
                            new Claim(ClaimTypes.Email, usuarioResponse.Email),
                            new Claim(nameof(usuarioResponse.Guid), usuarioResponse.Guid.ToString()),
                            new Claim(nameof(usuarioResponse.Colaborador.Guid), guidColaborador),
                            new Claim(nameof(usuarioResponse.Colaborador.Nome), nomeColaborador),
                            new Claim(nameof(usuarioResponse.Username), usuarioResponse.Username),
                            new Claim(nameof(usuarioResponse.Email), usuarioResponse.Email),
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