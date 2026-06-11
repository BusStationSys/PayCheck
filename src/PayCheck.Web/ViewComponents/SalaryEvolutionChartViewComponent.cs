namespace PayCheck.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
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

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryEvolutionChartViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <exception cref="Exception"></exception>
        public SalaryEvolutionChartViewComponent(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
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

            var guidUsuario = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponse.Guid)}Usuario").Value;

            var quantidadeMesesRetroativos = 6;

            var graficoEvolucaoSalarialResponse = default(
                IEnumerable<GraficoEvolucaoSalarialResponse>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"DemonstrativoPagamento/GraficoEvolucaoSalarial/{guidUsuario}/{quantidadeMesesRetroativos}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                        graficoEvolucaoSalarialResponse = JsonConvert.DeserializeObject<IEnumerable<GraficoEvolucaoSalarialResponse>>(
                            responseBody);
                }
                else
                {
                    if (responseBody.IsValidJson())
                    {
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(
                            responseBody);

                        ViewBag.ErrorMessage = problemDetails?.Detail ??
                            problemDetails?.Title ??
                            "Erro ao buscar notificações.";
                    }
                    else
                        ViewBag.ErrorMessage = "Erro desconhecido ao buscar notificações.";
                }
            }

            return View(
                this._mapper.Map<IEnumerable<GraficoEvolucaoSalarialViewModel>>(
                    graficoEvolucaoSalarialResponse));
        }
    }
}