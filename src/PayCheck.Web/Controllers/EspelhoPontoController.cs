namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System;
    using System.Net;

    public class EspelhoPontoController : Controller
    {
        private readonly Uri _baseAddress = new(
            Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EspelhoPontoController"/> class.
        /// </summary>
        public EspelhoPontoController()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
                cfg.CreateMap<MatriculaEspelhoPontoRequestUpdateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
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
                var authDto = new AuthDto
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

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                    authDtoJson);

                this._tokenBearer = authResponse.Token;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            var guidColaborador = default(Guid?);

            if (TempData.Peek("GuidColaborador") != null &&
                !string.IsNullOrEmpty(
                    TempData.Peek("GuidColaborador").ToString()))
            {
                guidColaborador = Guid.Parse(
                    TempData.Peek("GuidColaborador").ToString());
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto";

            if (guidColaborador != null)
                requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/getDemonstrativoPagamentoByGuidColaborador/{guidColaborador}";

            List<MatriculaEspelhoPontoResponse>? eps = null;

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                eps = JsonConvert.DeserializeObject<List<MatriculaEspelhoPontoResponse>>(stringJson);
            }

            return View(
                eps);
        }

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)
                return NotFound();

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/{guid}";

            var matriculaEspelhoPontoResponse = default(
                MatriculaEspelhoPontoResponse);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<MatriculaEspelhoPontoResponse>(stringJson);
            }

            return View(
                matriculaEspelhoPontoResponse); ;
        }

        private MatriculaEspelhoPontoResponse? GetMEP(Guid guid)
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/{guid}";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string matriculaEspelhoPontoResponseJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                return JsonConvert.DeserializeObject<MatriculaEspelhoPontoResponse>(
                    matriculaEspelhoPontoResponseJson);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IActionResult ConfirmarRealizacaoFrequencia(Guid guid)
        {
            var ipAddress = Common.GetIP();

            byte[] ipConfirmacao = null;

            if (ipAddress != null && ipAddress.GetAddressBytes != null)
                ipConfirmacao = ipAddress.GetAddressBytes();

            var matriculaEspelhoPontoResponse = this.GetMEP(
                guid);

            var matriculaEspelhoPontoRequestUpdateDto = this._mapper.Map<MatriculaEspelhoPontoRequestUpdateDto>(
                matriculaEspelhoPontoResponse);

            matriculaEspelhoPontoRequestUpdateDto.Guid = guid;
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

                matriculaEspelhoPontoRequestUpdateDtoJson = webApiHelper.ExecutePutWithAuthenticationByBearer(
                    matriculaEspelhoPontoRequestUpdateDtoJson);

                matriculaEspelhoPontoResponse = JsonConvert.DeserializeObject<MatriculaEspelhoPontoResponse>(
                    matriculaEspelhoPontoRequestUpdateDtoJson);
            }

            return RedirectToAction(
                "Details",
                new
                {
                    guid = matriculaEspelhoPontoResponse.Guid
                });
        }
    }
}