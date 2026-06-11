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

    public class SalaryCompositionChartViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SalaryCompositionChartViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <exception cref="Exception"></exception>
        public SalaryCompositionChartViewComponent(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
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

            var guidUsuario = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponse.Guid)}Usuario").Value;

            var competencia = "20230401";

            var graficoComposicaoSalarialResponse = default(
                IEnumerable<GraficoComposicaoSalarialResponse>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"DemonstrativoPagamento/GraficoComposicaoSalarial/{guidUsuario}/{competencia}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                        graficoComposicaoSalarialResponse = JsonConvert.DeserializeObject<IEnumerable<GraficoComposicaoSalarialResponse>>(
                            responseBody);
                }
            }

            return View(
                this._mapper.Map<IEnumerable<GraficoComposicaoSalarialViewModel>>(
                    graficoComposicaoSalarialResponse));
        }
    }
}