namespace PayCheck.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Models;
    using PayCheck.Web.Services.Interfaces;

    [Authorize]
    public class EspelhoPontoController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;


        /// <summary>
        /// Initializes a new instance of the <see cref="EspelhoPontoController"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public EspelhoPontoController(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"EspelhoPonto/{id}";

            var matriculaEspelhoPontoResponse = default(
                MatriculaEspelhoPontoResponseDto);

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                            dataJson).Data;
                }
            }

            return View(
                matriculaEspelhoPontoResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ConfirmarRealizacaoFrequencia(Guid id)
        {
            var tokenBearer = await this._authService.GetTokenAsync();

            var ipAddress = Common.GetIP();

            byte[] ipConfirmacao = null;

            if (ipAddress != null && ipAddress.GetAddressBytes != null)
                ipConfirmacao = ipAddress.GetAddressBytes();

            var matriculaEspelhoPontoResponse = await this.GetMEP(
                id);

            var matriculaEspelhoPontoRequestUpdateDto = this._mapper.Map<MatriculaEspelhoPontoRequestUpdateDto>(
                matriculaEspelhoPontoResponse);

            matriculaEspelhoPontoRequestUpdateDto.Guid = id;
            matriculaEspelhoPontoRequestUpdateDto.DataConfirmacao = DateTimeOffset.UtcNow;
            matriculaEspelhoPontoRequestUpdateDto.IpConfirmacao = ipConfirmacao;

            string requestUri = @$"EspelhoPonto/{matriculaEspelhoPontoRequestUpdateDto.Guid:N}";

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string content = JsonConvert.SerializeObject(
                matriculaEspelhoPontoRequestUpdateDto,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                });

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Put,
                requestUri,
                content))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                            dataJson).Data;
                }
            }

            return RedirectToAction(
                "Details",
                new
                {
                    id = matriculaEspelhoPontoResponse.Guid
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> GetDataTable()
        {
            var tokenBearer = await this._authService.GetTokenAsync();

            var guidColaborador = default(Guid?);

            if (TempData.Peek("GuidColaborador") != null &&
                !string.IsNullOrEmpty(
                    TempData.Peek("GuidColaborador").ToString()))
                guidColaborador = Guid.Parse(
                    TempData.Peek("GuidColaborador").ToString());

            string requestUri = "EspelhoPonto";

            if (guidColaborador != null)
                requestUri = @$"EspelhoPonto/Colaborador/{guidColaborador}";

            var espelhosPonto = default(IEnumerable<EspelhoPontoViewModel>);

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                    {
                        var source = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>>(
                            dataJson).Data;

                        espelhosPonto = this._mapper.Map<IEnumerable<EspelhoPontoViewModel>>(
                            source);
                    }
                }
            }

            //  Retrieve data from WebApi
            var query = from espelhoPonto in espelhosPonto
                        select new
                        {
                            espelhoPonto.Guid,
                            Competencia = string.Concat(
                                espelhoPonto.Competencia.Substring(4, 2),
                                "/",
                                espelhoPonto.Competencia.Substring(0, 4)),
                            espelhoPonto.NumeroMatricula,
                            Empregador = espelhoPonto.RazaoSocialEmpregador,
                            Colaborador = espelhoPonto.NomeColaborador,
                        };

            var draw = Request.Form["draw"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();

            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();

            var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? string.Empty;
            var start = Request.Form["start"].FirstOrDefault();

            //  Paging Size (10, 20, 50, 100)
            int pageSize = length != null ?
                Convert.ToInt32(
                    length) :
                    0;

            int skip = start != null ?
                Convert.ToInt32(
                    start) :
                    0;

            //  Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (!string.IsNullOrEmpty(sortColumnDir) &&
                    sortColumnDir.ToUpper() == "DESC")
                    query = query.OrderByDescending(pf => pf.GetType().GetProperty(
                        sortColumn).GetValue(
                            pf,
                            null));
                else
                    query = query.OrderBy(pf => pf.GetType().GetProperty(
                        sortColumn).GetValue(
                            pf,
                            null));
            }

            //  Search
            if (!string.IsNullOrEmpty(searchValue))
                query = query.Where(
                    td => td.Competencia.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                          td.NumeroMatricula.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.Empregador.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.Colaborador.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrEmpty(searchValue));

            //  Total Number of Rows Count.
            int recordsTotal = query.Count();

            //  Paging.
            var data = query.Skip(skip).Take(pageSize).ToList();

            // Create a JSON response with the data and total count.
            return new JsonResult(new
            {
                data,
                draw,
                recordsTotal,
                recordsFiltered = recordsTotal,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<MatriculaEspelhoPontoResponseDto?> GetMEP(Guid id)
        {
            var tokenBearer = await this._authService.GetTokenAsync();

            string requestUri = @$"EspelhoPonto/{id}";

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        return JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                            dataJson).Data;
                }
            }

            return null;
        }
    }
}