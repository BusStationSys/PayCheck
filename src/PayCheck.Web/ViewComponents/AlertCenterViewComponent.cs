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

    public class AlertCenterViewComponent : ViewComponent
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertCenterViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The service used to perform external HTTP calls required to retrieve alert data.</param>
        /// <param name="authService">The service responsible for providing authentication information for the current user.</param>
        /// <param name="mapper">The AutoMapper instance used to map data transfer objects to view models.</param>
        public AlertCenterViewComponent(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
        }

        /// <summary>
        /// Asynchronously invokes the user notifications view component.
        /// </summary>
        /// <remarks>
        /// This method retrieves notifications for the authenticated user by calling a protected API.
        /// The user must be authenticated in order for the notifications to be properly retrieved.
        /// </remarks>
        /// <returns>
        /// A view component result that displays the current user's notifications. The result may be empty
        /// if there are no notifications available.
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var claim = HttpContext.User.Claims.FirstOrDefault(
            //    c => c.Type == $"{nameof(UsuarioResponse.Guid)}Usuario");

            //if (claim is null)
            //{
            //    // aqui você decide: redirect, erro, etc.
            //    return Unauthorized();
            //}

            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var guid = HttpContext.User.Claims.First(
                c => c.Type == $"{nameof(UsuarioResponse.Guid)}Usuario").Value;

            var usuarioNotificacaoResponse = default(
                IEnumerable<UsuarioNotificacaoResponse>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"Usuario/Notificacoes/{guid}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                        usuarioNotificacaoResponse = JsonConvert.DeserializeObject<IEnumerable<UsuarioNotificacaoResponse>>(
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
                    {
                        ViewBag.ErrorMessage = "Erro desconhecido ao buscar notificações.";
                    }
                }
            }

            return View(
                this._mapper.Map<IEnumerable<UsuarioNotificacaoViewModel>>(
                    usuarioNotificacaoResponse));
        }
    }
}