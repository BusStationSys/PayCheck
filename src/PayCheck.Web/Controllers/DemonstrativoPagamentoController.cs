namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class DemonstrativoPagamentoController : Controller
    {
        private readonly Uri _baseAddress = new(
            Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemonstrativoPagamentoController"/> class.
        /// </summary>
        public DemonstrativoPagamentoController()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaDemonstrativoPagamentoRequestCreateDto, MatriculaDemonstrativoPagamentoResponseDto>().ReverseMap();
                cfg.CreateMap<MatriculaDemonstrativoPagamentoRequestUpdateDto, MatriculaDemonstrativoPagamentoResponseDto>().ReverseMap();
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

            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento";

            if (guidColaborador != null)
                requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento/getDemonstrativoPagamentoByGuidColaborador/{guidColaborador}";

            List<MatriculaDemonstrativoPagamentoResponseDto>? mdps = null;

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string matriculasDemonstrativosPagamentoResponseJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                mdps = JsonConvert.DeserializeObject<List<MatriculaDemonstrativoPagamentoResponseDto>>(
                    matriculasDemonstrativosPagamentoResponseJson);
            }

            return View(
                mdps);
        }

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)
            {
                return NotFound();
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento/{guid}";

            var mdp = default(MatriculaDemonstrativoPagamentoResponseDto);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                mdp = JsonConvert.DeserializeObject<MatriculaDemonstrativoPagamentoResponseDto>(stringJson);
            }

            return View(
                mdp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public IActionResult ConfirmarRecebimentoValores(Guid guid)
        {
            var ipAddress = Common.GetIP();

            byte[] ipConfirmacao = null;

            if (ipAddress != null && ipAddress.GetAddressBytes != null)
                ipConfirmacao = ipAddress.GetAddressBytes();

            var matriculaDemonstrativoPagamentoResponse = this.GetMDP(
                guid);

            var matriculaDemonstrativoPagamentoRequestUpdateDto = this._mapper.Map<MatriculaDemonstrativoPagamentoRequestUpdateDto>(
                matriculaDemonstrativoPagamentoResponse);

            matriculaDemonstrativoPagamentoRequestUpdateDto.Guid = guid;
            matriculaDemonstrativoPagamentoRequestUpdateDto.DataConfirmacao = DateTimeOffset.UtcNow;
            matriculaDemonstrativoPagamentoRequestUpdateDto.IpConfirmacao = ipConfirmacao;

            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento/{matriculaDemonstrativoPagamentoRequestUpdateDto.Guid:N}";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string matriculaDemonstrativoPagamentoRequestUpdateDtoJson = JsonConvert.SerializeObject(
                    matriculaDemonstrativoPagamentoRequestUpdateDto,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                matriculaDemonstrativoPagamentoRequestUpdateDtoJson = webApiHelper.ExecutePutWithAuthenticationByBearer(
                    matriculaDemonstrativoPagamentoRequestUpdateDtoJson);

                matriculaDemonstrativoPagamentoResponse = JsonConvert.DeserializeObject<MatriculaDemonstrativoPagamentoResponseDto>(
                    matriculaDemonstrativoPagamentoRequestUpdateDtoJson);
            }

            return RedirectToAction(
                "Details",
                new
                {
                    guid = matriculaDemonstrativoPagamentoResponse.Guid
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        private MatriculaDemonstrativoPagamentoResponseDto? GetMDP(Guid guid)
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento/{guid}";

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string matriculaDemonstrativoPagamentoResponseJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                return JsonConvert.DeserializeObject<MatriculaDemonstrativoPagamentoResponseDto>(
                    matriculaDemonstrativoPagamentoResponseJson);
            }
        }
    }
}