namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class PerfilController : Controller
    {
        private readonly Uri _baseAddress = new(Common.UriBaseApiString);

        private readonly HttpClient _httpClient;

        private readonly string _tokenBearer;

        public PerfilController()
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

                string authDtoJson = JsonConvert.SerializeObject(authDto,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                    });

                authDtoJson = webApiHelper.ExecutePostWithAuthenticationByBasic(
                    authDtoJson);

                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                    authDtoJson);

                //string stringJson = webApiHelper.ExecutePostAuthenticationByBasic(
                //    authDto);

                //var authResponse = JsonConvert.DeserializeObject<AuthResponse>(
                //    stringJson);

                this._tokenBearer = authResponse.Token;
            }
        }

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)   // Se não encontrar os Dados do Colaborador ou é porque não existe o registro ou é porque está logado como UserMain.
            {
                //return NotFound();
                return RedirectToAction("Index", "Home");   // Em não existindo o registro, redireciona para a página inicial.
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{guid}";

            var pf = default(PessoaFisicaResponse);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string stringJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                pf = JsonConvert.DeserializeObject<PessoaFisicaResponse>(stringJson);
            }

            return View(
                pf);
        }
    }
}