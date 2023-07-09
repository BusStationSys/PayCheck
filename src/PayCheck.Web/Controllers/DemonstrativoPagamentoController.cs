namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class DemonstrativoPagamentoController : Controller
    {
        private const string UriString = @"https://localhost:7104/api";

        private readonly Uri _baseAddress = new(UriString);

        private readonly HttpClient _httpClient;

        public DemonstrativoPagamentoController()
        {
            this._httpClient = new HttpClient
            {
                BaseAddress = this._baseAddress,
            };
        }

        [HttpGet]
        public IActionResult Index()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento";

            List<MatriculaDemonstrativoPagamentoResponse>? mdps = null;

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