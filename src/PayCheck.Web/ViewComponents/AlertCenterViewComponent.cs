namespace PayCheck.Web.ViewComponents
{
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using IdentityModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using PayCheck.Web.Models;
    using System.Security.Claims;

    public class AlertCenterViewComponent : ViewComponent
    {
        private readonly string _tokenBearer;

        private readonly ExternalApis _externalApis;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        private readonly Uri _baseAddress;

        public AlertCenterViewComponent(IOptions<ExternalApis> externalApis)
        {
            this._externalApis = externalApis.Value;

            this._baseAddress = new(
                this._externalApis.PayCheck);

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

        public Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimsPrincipal = HttpContext.User;

            //if (claimsPrincipal.Identity.IsAuthenticated)

            var guid = HttpContext.User.Claims.First(c => c.Type == $"{nameof(UsuarioResponseDto.Guid)}Usuario").Value;

            string requestUri = @$"{this._httpClient.BaseAddress}/Usuario/Notificacoes/{guid}";

            var usuarioNotificacaoResponse = default(
                IEnumerable<UsuarioNotificacaoResponseDto>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                    usuarioNotificacaoResponse = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<UsuarioNotificacaoResponseDto>>>(
                        dataJson).Data;
            }

            return Task.FromResult<IViewComponentResult>(View(
                this._mapper.Map<IEnumerable<UsuarioNotificacaoViewModel>>(usuarioNotificacaoResponse)));

            //this._httpClient.DefaultRequestHeaders.Clear();
            //this._httpClient.DefaultRequestHeaders.Add("Authorization",
            //    string.Concat("Bearer ", this._tokenBearer));
            //var response = this._httpClient.GetAsync(
            //    string.Concat(
            //        this._baseAddress,
            //        "/matriculas/1/demonstrativos-pagamento")).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var responseContent = response.Content.ReadAsStringAsync().Result;
            //    var demonstrativosPagamentoDto = JsonConvert.DeserializeObject<List<MatriculaDemonstrativoPagamentoResponseDto>>(
            //        responseContent);
            //    var demonstrativosPagamentoViewModel = this._mapper.Map<List<DemonstrativoPagamentoViewModel>>(
            //        demonstrativosPagamentoDto);
            //    return View(demonstrativosPagamentoViewModel);
            //}
            //else
            //{
            //    return View(new List<DemonstrativoPagamentoViewModel>());
            //}
        }
    }
}