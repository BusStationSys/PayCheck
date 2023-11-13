namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class ColaboradorController : Controller
    {
        private readonly string _tokenBearer;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColaboradorController"/> class.
        /// </summary>
        public ColaboradorController(IOptions<ExternalApis> externalApis)
        {
            var externalApisValue = externalApis.Value;

            Uri baseAddress = new(
                externalApisValue.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaResponseDto>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaResponseDto>().ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

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

        [HttpGet()]
        public IActionResult Index()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica";

            List<PessoaFisicaResponseDto>? data = null;

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    data = JsonConvert.DeserializeObject<ApiResponseDto<List<PessoaFisicaResponseDto>>>(
                        dataJson).Data;
            }

            return View(
                data);
        }

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)
                return NotFound();

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{guid}";

            var data = default(PessoaFisicaResponseDto);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    data = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(
                        dataJson).Data;
            }

            return View(
                data);
        }
    }
}