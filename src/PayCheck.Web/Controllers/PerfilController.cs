namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class PerfilController : Controller
    {
        private readonly string _tokenBearer;

        private readonly ExternalApis _externalApis;

        private readonly HttpClient _httpClient;

        private readonly Uri _baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfilController"/> class.
        /// </summary>
        /// <param name="externalApis"></param>
        public PerfilController(IOptions<ExternalApis> externalApis)
        {
            this._externalApis = externalApis.Value;

            this._baseAddress = new(
                this._externalApis.PayCheck);

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

        [HttpGet()]
        public IActionResult Details(Guid? guid)
        {
            if (guid == null)   // Se não encontrar os Dados do Colaborador ou é porque não existe o registro ou é porque está logado como UserMain.
            {
                //return NotFound();
                return RedirectToAction("Index", "Home");   // Em não existindo o registro, redireciona para a página inicial.
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{guid}";

            var pf = default(PessoaFisicaResponseDto);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string pessoaFisicaJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                pf = JsonConvert.DeserializeObject<PessoaFisicaResponseDto>(
                    pessoaFisicaJson);
            }

            return View(
                pf);
        }
    }
}