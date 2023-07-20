namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;

    public class DemonstrativoPagamentoController : Controller
    {
        private readonly Uri _baseAddress = new(Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        public DemonstrativoPagamentoController()
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = this._baseAddress,
            };

            using (var webApiHelper = new WebApiHelper(string.Concat(this._baseAddress, "/auth"), "arvtech", "(@rV73Ch)"))
            {
                string stringJson = webApiHelper.ExecutePost(true);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(stringJson);

                this._tokenBearer = authResponse.Token;
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento";

            List<MatriculaDemonstrativoPagamentoResponse>? mdps = null;

            //this._httpClient.DefaultRequestHeaders.Accept.Clear();

            //this._httpClient.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue(
            //        Common.MediaTypes));

            //this._httpClient.DefaultRequestHeaders.Add(
            //    "Authorization",
            //    $"Bearer {authResponse.Token}");

            //this._httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            HttpResponseMessage httpResponseMessage = this._httpClient.GetAsync(
                requestUri).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string data = httpResponseMessage.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data))
                {
                    mdps = JsonConvert.DeserializeObject<List<MatriculaDemonstrativoPagamentoResponse>>(data);
                }
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

            MatriculaDemonstrativoPagamentoResponse mdp = null;

            HttpResponseMessage httpResponseMessage = this._httpClient.GetAsync(
                requestUri).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string data = httpResponseMessage.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data))
                {
                    mdp = JsonConvert.DeserializeObject<MatriculaDemonstrativoPagamentoResponse>(data);
                }
                else
                {
                    return NotFound();
                }
            }

            return View(
                mdp);
        }
    }
}