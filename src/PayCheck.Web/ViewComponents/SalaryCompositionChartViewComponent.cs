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
    using PayCheck.Web.Services.Interfaces;

    public class SalaryCompositionChartViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryCompositionChartViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <exception cref="Exception"></exception>
        public SalaryCompositionChartViewComponent(IHttpClientService httpClientService, IAuthService authService)
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

            this._authService = authService;
        }

        /// <summary>
        /// Invokes the user salary composition view component for the specified period.
        /// </summary>
        /// <remarks>
        /// This method retrieves salary composition data for the authenticated user for a fixed period
        /// and presents it in a view. The method depends on valid authentication and access to the payroll
        /// demonstration API. The result may vary depending on user permissions and data availability.
        /// </remarks>
        /// <returns>
        /// A view component result that displays the user's salary composition data. The result may be empty
        /// if there is no data available for the specified period.
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var guidUsuario = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            var competencia = "20230401";

            var graficoComposicaoSalarialResponse = default(
                IEnumerable<GraficoComposicaoSalarialResponseDto>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"DemonstrativoPagamento/GraficoComposicaoSalarial/{guidUsuario}/{competencia}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        graficoComposicaoSalarialResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>>>(
                            dataJson).Data;
                }
            }

            return View(
                this._mapper.Map<IEnumerable<GraficoComposicaoSalarialViewModel>>(
                    graficoComposicaoSalarialResponse));
        }
    }
}