using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayCheck.Web.Models;

namespace PayCheck.Web.Controllers
{
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
            string requestUri = @$"{this._httpClient.BaseAddress}/DemonstrativoPagamento/20230501/75800";

            List<DemonstrativoPagamentoViewModel>? demonstrativosPagamento = null;

            HttpResponseMessage httpResponseMessage = this._httpClient.GetAsync(
                requestUri).Result;

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string data = httpResponseMessage.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data))
                {
                    demonstrativosPagamento = JsonConvert.DeserializeObject<List<DemonstrativoPagamentoViewModel>>(data);
                }
            }

            return View(
                demonstrativosPagamento);
        }
    }
}
