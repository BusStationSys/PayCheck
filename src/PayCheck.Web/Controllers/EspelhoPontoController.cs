namespace PayCheck.Web.Controllers
{
    using System;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using PayCheck.Web.Models;

    [Authorize]
    public class EspelhoPontoController : Controller
    {
        private readonly string _tokenBearer;

        private readonly ExternalApis _externalApis;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        private readonly Uri _baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="EspelhoPontoController"/> class.
        /// </summary>
        /// <param name="externalApis"></param>
        public EspelhoPontoController(IOptions<ExternalApis> externalApis)
        {
            this._externalApis = externalApis.Value;

            this._baseAddress = new(
                this._externalApis.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponseDto>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoRequestUpdateDto, MatriculaEspelhoPontoResponseDto>().ReverseMap();

                cfg.CreateMap<MatriculaEspelhoPontoResponseDto, EspelhoPontoViewModel>()
                    .ForMember(
                        dest => dest.NumeroMatricula,
                        opt => opt.MapFrom(
                            src => src.Matricula.Matricula))
                    .ForMember(
                        dest => dest.NomeColaborador,
                        opt => opt.MapFrom(
                            src => src.Matricula.Colaborador.Nome))
                    .ForMember(
                        dest => dest.RazaoSocialEmpregador,
                        opt => opt.MapFrom(
                            src => src.Matricula.Empregador.RazaoSocial)).ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

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
        public IActionResult Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/{id}";

            var matriculaEspelhoPontoResponse = default(
                MatriculaEspelhoPontoResponseDto);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                        dataJson).Data;
            }

            return View(
                matriculaEspelhoPontoResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IActionResult ConfirmarRealizacaoFrequencia(Guid id)
        {
            var ipAddress = Common.GetIP();

            byte[] ipConfirmacao = null;

            if (ipAddress != null && ipAddress.GetAddressBytes != null)
                ipConfirmacao = ipAddress.GetAddressBytes();

            var matriculaEspelhoPontoResponse = this.GetMEP(
                id);

            var matriculaEspelhoPontoRequestUpdateDto = this._mapper.Map<MatriculaEspelhoPontoRequestUpdateDto>(
                matriculaEspelhoPontoResponse);

            matriculaEspelhoPontoRequestUpdateDto.Guid = id;
            matriculaEspelhoPontoRequestUpdateDto.DataConfirmacao = DateTimeOffset.UtcNow;
            matriculaEspelhoPontoRequestUpdateDto.IpConfirmacao = ipConfirmacao;

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/{matriculaEspelhoPontoRequestUpdateDto.Guid:N}";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string matriculaEspelhoPontoRequestUpdateDtoJson = JsonConvert.SerializeObject(
                    matriculaEspelhoPontoRequestUpdateDto,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                string dataJson = webApiHelper.ExecutePutWithAuthenticationByBearer(
                    matriculaEspelhoPontoRequestUpdateDtoJson);

                if (dataJson.IsValidJson())
                    matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                        dataJson).Data;
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
        public IActionResult GetDataTable()
        {
            var guidColaborador = default(Guid?);

            if (TempData.Peek("GuidColaborador") != null &&
                !string.IsNullOrEmpty(
                    TempData.Peek("GuidColaborador").ToString()))
                guidColaborador = Guid.Parse(
                    TempData.Peek("GuidColaborador").ToString());

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto";

            if (guidColaborador != null)
                requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/getEspelhoPontoByGuidColaborador/{guidColaborador}";

            var espelhosPonto = default(IEnumerable<EspelhoPontoViewModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var source = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>>(
                        dataJson).Data;

                    espelhosPonto = this._mapper.Map<IEnumerable<EspelhoPontoViewModel>>(
                        source);
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
        private MatriculaEspelhoPontoResponseDto? GetMEP(Guid id)
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/{id}";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    return JsonConvert.DeserializeObject<ApiResponseDto<MatriculaEspelhoPontoResponseDto>>(
                        dataJson).Data;

                return null;
            }
        }
    }
}