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

    public class SalaryEvolutionChartViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryEvolutionChartViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <exception cref="Exception"></exception>
        public SalaryEvolutionChartViewComponent(IHttpClientService httpClientService, IAuthService authService)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<GraficoEvolucaoSalarialResponseDto, GraficoEvolucaoSalarialViewModel>()
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
        /// Invokes the view component asynchronously to retrieve and display the user's salary evolution chart for the
        /// past six months.
        /// </summary>
        /// <remarks>This method retrieves the current user's identifier from the HTTP context, fetches
        /// salary evolution data for the last six months from an external API, and maps the result to the view model
        /// for rendering. The method requires the user to be authenticated and may return an empty chart if no data is
        /// available.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="IViewComponentResult"/> that renders the salary evolution chart view with the relevant data.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var guidUsuario = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            var quantidadeMesesRetroativos = 6;

            var graficoEvolucaoSalarialResponse = default(
                IEnumerable<GraficoEvolucaoSalarialResponseDto>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"DemonstrativoPagamento/GraficoEvolucaoSalarial/{guidUsuario}/{quantidadeMesesRetroativos}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    //var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    //graficoEvolucaoSalarialResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>>>(
                    //    responseJson).Data;

                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        graficoEvolucaoSalarialResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>>>(
                            dataJson).Data;
                }
            }

            return View(
                this._mapper.Map<IEnumerable<GraficoEvolucaoSalarialViewModel>>(
                    graficoEvolucaoSalarialResponse));
        }
    }
}