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

    public class AlertCenterViewComponent : ViewComponent
    {
        private readonly string _tokenBearer;

        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertCenterViewComponent"/> class.
        /// </summary>
        /// <param name="httpClientService">The service used to perform external HTTP calls required to retrieve alert data.</param>
        /// <param name="authService">The service responsible for providing authentication information for the current user.</param>
        public AlertCenterViewComponent(IHttpClientService httpClientService, IAuthService authService)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MatriculaDemonstrativoPagamentoResponseDto, DemonstrativoPagamentoViewModel>()
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

                cfg.CreateMap<UsuarioNotificacaoResponseDto, UsuarioNotificacaoViewModel>().ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

            this._httpClientService = httpClientService;

            this._authService = authService;
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
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            var guid = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            var usuarioNotificacaoResponse = default(
                IEnumerable<UsuarioNotificacaoResponseDto>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = @$"Usuario/Notificacoes/{guid}";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                        usuarioNotificacaoResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>>>(
                            dataJson).Data;
                }
            }

            return View(
                this._mapper.Map<IEnumerable<UsuarioNotificacaoViewModel>>(
                    usuarioNotificacaoResponse));
        }
    }
}