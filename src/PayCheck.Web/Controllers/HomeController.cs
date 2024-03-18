namespace PayCheck.Web.Controllers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using ARVTech.DataAccess.DTOs;
using ARVTech.DataAccess.DTOs.UniPayCheck;
using ARVTech.Shared.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PayCheck.Web.Models;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly string _tokenBearer;

    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IOptions<ExternalApis> externalApis)
    {
        this._logger = logger;

        var externalApisValue = externalApis.Value;

        Uri baseAddress = new(
            externalApisValue.PayCheck);

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

        ViewData["Aniversariantes"] = this.LoadAniversariantes(
            DateTime.Now,
            DateTime.Now);

        ViewData["AniversariantesEmpresa"] = this.LoadAniversariantesEmpresa(
            DateTime.Now.Month);

        ViewData["SobreNos"] = this.LoadSobreNos(
            DateTime.Now);

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

    private IEnumerable<dynamic> LoadAniversariantes(DateTime periodoInicial, DateTime periodoFinal)
    {
        string periodoInicialString = periodoInicial.ToString("MMdd");
        string periodoFinalString = periodoFinal.ToString("MMdd");

        string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/getAniversariantes/{periodoInicialString}/{periodoFinalString}";

        var pessoasFisicas = default(IEnumerable<PessoaFisicaResponseDto>);

        using (var webApiHelper = new WebApiHelper(
            requestUri,
            this._tokenBearer))
        {
            string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

            if (dataJson.IsValidJson())
                pessoasFisicas = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>>(
                    dataJson).Data;
        }

        if (pessoasFisicas != null &&
            pessoasFisicas.Count() > 0)
        {
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
                a => a.DataNascimentoOrdenado)
                    .ThenBy(a => a.Nome).ToList();
        }

        return Enumerable.Empty<dynamic>();
    }

    private IEnumerable<dynamic> LoadAniversariantesEmpresa(int mes)
    {
        string requestUri = @$"{this._httpClient.BaseAddress}/Matricula/getAniversariantesEmpresa/{mes}";

        var matriculas = default(IEnumerable<MatriculaResponseDto>);

        using (var webApiHelper = new WebApiHelper(
            requestUri,
            this._tokenBearer))
        {
            string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

            if (dataJson.IsValidJson())
            {
                matriculas = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<MatriculaResponseDto>>>(
                    dataJson).Data;
            }
        }

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
            a => a.DataAdmissaoOrdenada)
                .ThenByDescending(a => a.DataAdmissao)
                .ThenBy(a => a.Nome).ToList();
    }

    private IEnumerable<dynamic> LoadSobreNos(DateTime dataAtual)
    {
        string dataAtualString = dataAtual.ToString("yyyy-MM-dd");

        string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao/getSobreNos/{dataAtualString}";

        var publicacoes = default(IEnumerable<PublicacaoResponseDto>);

        using (var webApiHelper = new WebApiHelper(
            requestUri,
            this._tokenBearer))
        {
            string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

            if (dataJson.IsValidJson())
            {
                publicacoes = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PublicacaoResponseDto>>>(
                    dataJson).Data;
            }
        }

        if (publicacoes != null &&
            publicacoes.Count() > 0)
        {
            var sobreNos = from sn in publicacoes
                           select new
                           {
                               sn.Id,
                               sn.Resumo,
                               sn.Titulo,
                               sn.Texto,
                               sn.ConteudoImagem,
                               sn.ExtensaoImagem,
                           };

            return sobreNos.ToList();
        }

        return Enumerable.Empty<dynamic>();
    }

    public IActionResult RenderImage(int id)
    {
        string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao/getImage/{id}";

        var publicacao = default(PublicacaoResponseDto);

        using (var webApiHelper = new WebApiHelper(
            requestUri,
            this._tokenBearer))
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