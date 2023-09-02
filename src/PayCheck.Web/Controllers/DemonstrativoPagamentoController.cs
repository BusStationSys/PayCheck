namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class DemonstrativoPagamentoController : Controller
    {
        private readonly Uri _baseAddress = new(
            Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        /// <summary>
        /// 
        /// </summary>
        public DemonstrativoPagamentoController()
        {
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
                var authDto = new AuthDto
                {
                    Username = "arvtech",
                    Password = "(@rV73Ch)",
                };

                //string stringJson = webApiHelper.ExecutePostAuthenticationByBasic(
                //    authDto);

                //var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                //    stringJson);

                string authDtoJson = JsonConvert.SerializeObject(authDto,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                authDtoJson = webApiHelper.ExecutePostAuthenticationByBasic(
                    authDtoJson);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
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

            List<MatriculaDemonstrativoPagamentoResponse>? mdps = null;

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetAuthenticationByBearer();

                mdps = JsonConvert.DeserializeObject<List<MatriculaDemonstrativoPagamentoResponse>>(stringJson);
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

            var mdp = default(MatriculaDemonstrativoPagamentoResponse);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetAuthenticationByBearer();

                mdp = JsonConvert.DeserializeObject<MatriculaDemonstrativoPagamentoResponse>(stringJson);
            }

            return View(
                mdp);
        }
    }
}