namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class EspelhoPontoController : Controller
    {
        private readonly Uri _baseAddress = new(
            Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        public EspelhoPontoController()
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

                string stringJson = webApiHelper.ExecutePostAuthenticationByBasic(
                    authDto);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                    stringJson);

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

            string requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto";

            if (guidColaborador != null)
                requestUri = @$"{this._httpClient.BaseAddress}/EspelhoPonto/getDemonstrativoPagamentoByGuidColaborador/{guidColaborador}";

            List<MatriculaEspelhoPontoResponse>? eps = null;

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetAuthenticationByBearer();

                eps = JsonConvert.DeserializeObject<List<MatriculaEspelhoPontoResponse>>(stringJson);
            }

            return View(
                eps);
        }

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)
            {
                return NotFound();
            }

            return null;
        }
    }
}