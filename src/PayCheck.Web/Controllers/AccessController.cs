namespace PayCheck.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Claims;
    using System.Text;
    using System.Web;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using ARVTech.Shared.Email;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using PayCheck.Web.Models;

    public class AccessController : Controller
    {
        private readonly string _tokenBearer;

        private readonly IEmailService _emailService;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessController"/> class.
        /// </summary>
        /// <param name="emailService"></param>
        /// <param name="externalApis"></param>
        public AccessController(IEmailService emailService, IOptions<ExternalApis> externalApis)
        {
            //this._baseAddress = new(
            //    externalApis.Value.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LoginRequestDto, LoginViewModel>().ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

            Uri baseAddress = new(
                externalApis.Value.PayCheck);

            this._httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };

            using (var webApiHelper = new WebApiHelper(
                string.Concat(
                    baseAddress,
                    "/auth"),
                "arvtech",
                "(@rV73Ch)"))
            {
                var authDto = new AuthRequestDto
                {
                    Username = "arvtech",
                    Password = "(@rV73Ch)",
                };

                string authDtoJson = JsonConvert.SerializeObject(authDto,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                authDtoJson = webApiHelper.ExecutePostWithAuthenticationByBasic(
                    authDtoJson);

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(
                    authDtoJson);

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
        public IActionResult ChangePassword(AlteracaoSenhaRequestDto alteracaoSenhaDto)
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

                var usuarioResponse = default(UsuarioResponseDto);

                string requestUri = @$"{this._httpClient.BaseAddress}/Usuario/{usuarioUpdateDto.Guid:N}";

                using (var webApiHelper = new WebApiHelper(
                    requestUri,
                    this._tokenBearer))
                {
                    string usuarioUpdateDtoJson = JsonConvert.SerializeObject(
                        usuarioUpdateDto,
                        Formatting.None,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        });

                    usuarioUpdateDtoJson = webApiHelper.ExecutePutWithAuthenticationByBearer(
                        usuarioUpdateDtoJson);

                    usuarioResponse = JsonConvert.DeserializeObject<UsuarioResponseDto>(
                        usuarioUpdateDtoJson);
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

                    var parametros = $"GuidUsuario={guidUsuario:N}&DataAtual={dataAtualString}&Email={activateViewModel.Email}&GuidColaborador={guidColaborador:N}&Username={username}";

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
                    modelStateErrors.Count() > 0)
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

            var loginDto = this._mapper.Map<LoginRequestDto>(
                vm);

            string requestUri = @$"{this._httpClient.BaseAddress}/Usuario";

            string fromBodyString = JsonConvert.SerializeObject(
                loginDto,
                Formatting.Indented);

            //  string fromBodyString = JsonConvert.SerializeObject(loginDto,
            //        Formatting.None,
            //      new JsonSerializerSettings
            //      {
            //          NullValueHandling = NullValueHandling.Ignore,
            //      });

            var apiResponseDto = default(ApiResponseDto<UsuarioResponseDto>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                fromBodyString = webApiHelper.ExecutePostWithAuthenticationByBearer(
                    fromBodyString);

                if (fromBodyString.IsValidJson())
                    apiResponseDto = JsonConvert.DeserializeObject<ApiResponseDto<UsuarioResponseDto>>(
                        fromBodyString);
            }

            if (apiResponseDto != null &&
                apiResponseDto.Success)
            {
                var usuarioResponse = apiResponseDto.Data;

                if (usuarioResponse.DataPrimeiroAcesso is null ||
                    !usuarioResponse.DataPrimeiroAcesso.HasValue)
                {
                    TempData.Remove("GuidUsuario");
                    TempData.Remove("GuidColaborador");
                    TempData.Remove("NomeColaborador");
                    TempData.Remove("EmailColaborador");
                    TempData.Remove("Username");

                    TempData["GuidUsuario"] = usuarioResponse.Guid;
                    TempData["GuidColaborador"] = usuarioResponse.Colaborador.Guid;
                    TempData["NomeColaborador"] = usuarioResponse.Colaborador.Nome;
                    TempData["EmailColaborador"] = usuarioResponse.Colaborador.Pessoa.Email;
                    TempData["Username"] = usuarioResponse.Username;

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
                            new Claim(ClaimTypes.NameIdentifier, usuarioResponse.Guid.ToString()),
                            new Claim(ClaimTypes.Name, nomeColaborador),
                            new Claim(ClaimTypes.Email, emailUsuario),

                            new Claim(
                                $"{nameof(
                                    UsuarioResponseDto.Guid)}Usuario",
                                usuarioResponse.Guid.ToString()),

                            new Claim(
                                $"{nameof(
                                    UsuarioResponseDto.Colaborador.Guid)}Colaborador",
                                guidColaborador),

                            new Claim(
                                $"{nameof(
                                    UsuarioResponseDto.Colaborador.Nome)}Colaborador",
                                nomeColaborador),

                            new Claim(
                                nameof(
                                    UsuarioResponseDto.Username),
                                usuarioResponse.Username),

                            new Claim(
                                $"{nameof(UsuarioResponseDto.Email)}Usuario",
                                emailUsuario),

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
                ViewBag.ErrorMessage = $"<p>{apiResponseDto.Message}</p>";

            vm.Password = string.Empty;

            return View(
                vm);
        }
    }
}