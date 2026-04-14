namespace PayCheck.Web.ViewComponents
{
    using System.Security.Claims;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Models;

    public class SalaryCompositionChartViewComponent : ViewComponent
    {
        private readonly string _tokenBearer;

        private readonly IHttpClientService _httpClientService;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryCompositionChartViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <exception cref="Exception"></exception>
        public SalaryCompositionChartViewComponent(IHttpClientService httpClientService)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GraficoComposicaoSalarialResponseDto, GraficoComposicaoSalarialViewModel>()
                    .ForMember(
                        dest => dest.Competencia,
                        opt => opt.MapFrom(
                            src => src.CompetenciaFormatada)).ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

            this._httpClientService = httpClientService;

            //using (var webApiHelper = new WebApiHelper(
            //    string.Concat(
            //        this._baseAddress,
            //        "/auth"),
            //    "arvtech",
            //    "(@rV73Ch)"))
            //{
            //    var authDto = new AuthRequestDto
            //    {
            //        Username = "arvtech",
            //        Password = "(@rV73Ch)",
            //    };

            //    string authDtoJson = JsonConvert.SerializeObject(authDto,
            //        Formatting.None,
            //        new JsonSerializerSettings
            //        {
            //            NullValueHandling = NullValueHandling.Ignore,
            //        });

            //    authDtoJson = webApiHelper.ExecutePostWithAuthenticationByBasic(
            //        authDtoJson);

            //    var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(
            //        authDtoJson);

            //    this._tokenBearer = authResponse.Token;
            //}

            var authDto = new AuthRequestDto
            {
                Username = "arvtech",
                Password = "(@rV73Ch)"
            };

            var json = JsonConvert.SerializeObject(
                authDto,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            // 🔐 Basic Auth (igual ao que o WebApiHelper fazia)
            this._httpClientService.SetBasicAuthentication("arvtech", "(@rV73Ch)");

            using (var httpResponseMessage = this._httpClientService.ExecuteAsync(
                HttpMethod.Post,
                "auth",
                json).GetAwaiter().GetResult())
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception("Erro ao autenticar.");

                var responseJson = httpResponseMessage.Content
                    .ReadAsStringAsync()
                    .GetAwaiter()
                    .GetResult();

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(responseJson);

                this._tokenBearer = authResponse.Token;
            }
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var guidUsuario = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            var competencia = "20230401";

            string requestUri = @$"DemonstrativoPagamento/GraficoComposicaoSalarial/{guidUsuario}/{competencia}";

            var graficoComposicaoSalarialResponse = default(
                IEnumerable<GraficoComposicaoSalarialResponseDto>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    graficoComposicaoSalarialResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>>>(
                        dataJson).Data;
            }

            return Task.FromResult<IViewComponentResult>(View(
                this._mapper.Map<IEnumerable<GraficoComposicaoSalarialViewModel>>(graficoComposicaoSalarialResponse)));
        }
    }
}