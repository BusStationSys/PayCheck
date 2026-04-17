namespace PayCheck.Web.Controllers;

using ARVTech.DataAccess.DTOs;
using ARVTech.DataAccess.DTOs.UniPayCheck;
using ARVTech.Shared.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayCheck.Web.Infrastructure.Http.Interfaces;
using PayCheck.Web.Models;
using PayCheck.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //  private readonly string _tokenBearer;

    private readonly IHttpClientService _httpClientService;

    private readonly IAuthService _authService;

    private readonly Mapper _mapper;

    public HomeController(ILogger<HomeController> logger, IHttpClientService httpClientService, IAuthService authService)
    {
        this._logger = logger;

        var mapperConfiguration = new MapperConfiguration(cfg =>
        {
            //cfg.CreateMap<PublicacaoRequestCreateDto, PublicacaoResponseDto>().ReverseMap();
            //cfg.CreateMap<PublicacaoRequestUpdateDto, PublicacaoResponseDto>().ReverseMap();
            //cfg.CreateMap<PublicacaoRequestCreateDto, PublicacaoModel>().ReverseMap();
            //cfg.CreateMap<PublicacaoRequestUpdateDto, PublicacaoModel>().ReverseMap();

            cfg.CreateMap<PublicacaoResponseDto, PublicacaoModel>().ReverseMap();
        });

        this._mapper = new Mapper(
            mapperConfiguration);

        this._httpClientService = httpClientService;

        this._authService = authService;

        //var authDto = new AuthRequestDto
        //{
        //    Username = "arvtech",
        //    Password = "(@rV73Ch)"
        //};

        //var json = JsonConvert.SerializeObject(
        //    authDto,
        //    Formatting.None,
        //    new JsonSerializerSettings
        //    {
        //        NullValueHandling = NullValueHandling.Ignore
        //    });

        //// 🔐 Basic Auth (igual ao que o WebApiHelper fazia)
        //this._httpClientService.SetBasicAuthentication("arvtech", "(@rV73Ch)");

        //using (var httpResponseMessage = this._httpClientService.ExecuteAsync(
        //    HttpMethod.Post,
        //    "auth",
        //    json).GetAwaiter().GetResult())
        //{
        //    if (!httpResponseMessage.IsSuccessStatusCode)
        //        throw new Exception("Erro ao autenticar.");

        //    var responseJson = httpResponseMessage.Content
        //        .ReadAsStringAsync()
        //        .GetAwaiter()
        //        .GetResult();

        //    var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);

        //    this._tokenBearer = authResponse.Token;
        //}
    }

    public IActionResult Index()
    {
        ClaimsPrincipal claimsPrincipal = HttpContext.User;

        if (claimsPrincipal.Identity.IsAuthenticated)
        {
            TempData["GuidUsuario"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            TempData["Username"] = HttpContext.User.Claims.First(c => c.Type == nameof(UsuarioResponseDto.Username)).Value;

            TempData["GuidColaborador"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Colaborador.Guid)}Colaborador").Value;

            TempData["NomeColaborador"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Colaborador.Nome)}Colaborador").Value;

            TempData["EmailUsuario"] = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Email)}Usuario").Value;

            TempData.Keep();
        }

        ViewData["Aniversariantes"] = this.LoadAniversariantesAsync(
            DateTime.Now,
            DateTime.Now);

        ViewData["AniversariantesEmpresa"] = this.LoadAniversariantesEmpresaAsync(
            DateTime.Now.Month);

        //ViewData["SobreNos"] = this.LoadSobreNosAsync(
        //    DateTime.Now);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(
            "Login",
            "Access");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<IEnumerable<dynamic>> LoadAniversariantesAsync(DateTime periodoInicial, DateTime periodoFinal)
    {
        var pessoasFisicas = default(IEnumerable<PessoaFisicaResponseDto>);

        var tokenBearer = await this._authService.GetTokenAsync();

        string periodoInicialString = periodoInicial.ToString("MMdd");
        string periodoFinalString = periodoFinal.ToString("MMdd");

        //  Inicia o HttpClientSingleton de consumo da API.
        this._httpClientService.SetBearerAuthentication(
            tokenBearer);

        string requestUri = $"PessoaFisica/Aniversariantes?periodoInicialString={periodoInicialString}&periodoFinalString={periodoFinalString}";

        using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
            HttpMethod.Get,
            requestUri))
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                //if (dataJson.IsValidJson())
                //{
                //    matriculas = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<MatriculaResponseDto>>>(
                //        dataJson).Data;
                //}
            }
        }

        if (pessoasFisicas is null ||
            !pessoasFisicas.Any())
            return Enumerable.Empty<dynamic>();

        var aniversariantes = from aniversariante in pessoasFisicas
                              where aniversariante.DataNascimento != null
                              select new
                              {
                                  aniversariante.Guid,
                                  aniversariante.Nome,
                                  DataNascimentoOrdenado = Convert.ToDateTime(
                                      aniversariante.DataNascimento).ToString("MMdd"),
                                  DataNascimentoString = Convert.ToDateTime(
                                      aniversariante.DataNascimento).ToString("dd/MM"),
                                  Indice = Convert.ToDouble(
                                      Math.Round(
                                          (Convert.ToDateTime(
                                              Convert.ToDateTime(
                                                  aniversariante.DataNascimento).ToString("dd/MM") + "/" + DateTime.Now.Year) - DateTime.Now).TotalDays, 2)),
                              };

        return aniversariantes.OrderBy(
            a => a.DataNascimentoOrdenado).ThenBy(
                a => a.Nome).ToList();
    }

    private async Task<IEnumerable<dynamic>> LoadAniversariantesEmpresaAsync(int mes)
    {
        var matriculas = default(IEnumerable<MatriculaResponseDto>);

        var tokenBearer = await this._authService.GetTokenAsync();

        //  Inicia o HttpClientSingleton de consumo da API.
        this._httpClientService.SetBearerAuthentication(
            tokenBearer);

        string requestUri = @$"Matricula/AniversariantesEmpresa/{mes}";

        using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
            HttpMethod.Get,
            requestUri))
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                if (dataJson.IsValidJson())
                {
                    matriculas = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<MatriculaResponseDto>>>(
                        dataJson).Data;
                }
            }
        }

        if (matriculas is null ||
            !matriculas.Any())
            return Enumerable.Empty<dynamic>();

        var aniversariantes = from aniversarianteEmpresa in matriculas
                              select new
                              {
                                  aniversarianteEmpresa.Guid,
                                  aniversarianteEmpresa.Colaborador.Nome,
                                  DataAdmissaoOrdenada = Convert.ToDateTime(
                                      aniversarianteEmpresa.DataAdmissao).ToString("MMdd"),
                                  DataAdmissaoString = Convert.ToDateTime(
                                      aniversarianteEmpresa.DataAdmissao).ToString("dd/MM"),
                                  aniversarianteEmpresa.DataAdmissao,
                                  Indice = Convert.ToDouble(
                                      Math.Round(
                                          (Convert.ToDateTime(
                                              Convert.ToDateTime(
                                                  aniversarianteEmpresa.DataAdmissao).ToString("dd/MM") + "/" + DateTime.Now.Year) - DateTime.Now).TotalDays, 2)),
                              };

        return aniversariantes.OrderBy(
            a => a.DataAdmissaoOrdenada).ThenByDescending(
                a => a.DataAdmissao).ThenBy(
            a => a.Nome).ToList();
    }

    //private IEnumerable<dynamic> LoadSobreNosAsync(DateTime dataAtual)
    //{
    //    var tokenBearer = this._authService.GetTokenAsync().Result;

    //    string dataAtualString = dataAtual.ToString("yyyy-MM-dd");

    //    string requestUri = @$"Publicacao/getSobreNos/{dataAtualString}";

    //    var publicacoes = default(IEnumerable<PublicacaoResponseDto>);

    //    using (var webApiHelper = new WebApiHelper(
    //        requestUri,
    //        tokenBearer))
    //    {
    //        string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

    //        if (dataJson.IsValidJson())
    //        {
    //            publicacoes = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PublicacaoResponseDto>>>(
    //                dataJson).Data;
    //        }
    //    }

    //    if (publicacoes != null &&
    //        publicacoes.Count() > 0)
    //    {
    //        var sobreNos = from sn in publicacoes
    //                       select new
    //                       {
    //                           sn.Id,
    //                           sn.Resumo,
    //                           sn.Titulo,
    //                           sn.Texto,
    //                           sn.ConteudoImagem,
    //                           sn.ExtensaoImagem,
    //                       };

    //        return sobreNos.ToList();
    //    }

    //    return Enumerable.Empty<dynamic>();
    //}

    //public async Task<IActionResult> SobreNos(int? id)
    //{
    //    if (id is null)
    //        return NotFound();

    //    var tokenBearer = await this._authService.GetTokenAsync();

    //    string requestUri = @$"Publicacao/{id}";

    //    var publicacao = default(PublicacaoModel);

    //    using (var webApiHelper = new WebApiHelper(
    //        requestUri,
    //        tokenBearer))
    //    {
    //        string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

    //        if (dataJson.IsValidJson())
    //        {
    //            var data = JsonConvert.DeserializeObject<ApiResponseDto<PublicacaoResponseDto>>(
    //                dataJson).Data;

    //            publicacao = this._mapper.Map<PublicacaoModel>(
    //                data);
    //        }
    //    }

    //    return View(
    //        publicacao);
    //}

    public IActionResult RenderImage(int id)
    {
        var tokenBearer = this._authService.GetTokenAsync().Result;

        string requestUri = @$"Publicacao/getImage/{id}";

        var publicacao = default(PublicacaoResponseDto);

        using (var webApiHelper = new WebApiHelper(
            requestUri,
            tokenBearer))
        {
            string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

            if (dataJson.IsValidJson())
                publicacao = JsonConvert.DeserializeObject<ApiResponseDto<PublicacaoResponseDto>>(
                    dataJson).Data;
        }

        if (publicacao != null)
        {
            string contentType = "image/png";

            if (publicacao.ExtensaoArquivo == "bmp")
                contentType = "image/bmp";
            else if (publicacao.ExtensaoArquivo == "gif")
                contentType = "image/gif";
            else if (publicacao.ExtensaoArquivo == "jpeg" ||
                publicacao.ExtensaoArquivo == "jpg")
                contentType = "image/jpeg";
            else if (publicacao.ExtensaoArquivo == "svg")
                contentType = "image/svg+xml";

            var base64String = Convert.ToBase64String(
                publicacao.ConteudoImagem);

            var arrayImage = Convert.FromBase64String(
                base64String);

            return this.File(
                arrayImage,
                contentType,
                publicacao.NomeImagem);
        }

        return null;
    }
}