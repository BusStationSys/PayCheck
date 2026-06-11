namespace PayCheck.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Create;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Models;
    using PayCheck.Web.Services.Interfaces;

    [Authorize]
    public class EmpregadorController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpregadorController"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public EmpregadorController(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Index()
        {
            var pessoasJuridicas = default(IEnumerable<PessoaJuridicaViewModel>);

            var tokenBearer = await this._authService.GetTokenAsync();

            string requestUri = "PessoaJuridica";

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                    {
                        var source = JsonConvert.DeserializeObject<IEnumerable<PessoaJuridicaResponse>>(
                            responseBody);

                        pessoasJuridicas = this._mapper.Map<IEnumerable<PessoaJuridicaViewModel>>(
                            source);
                    }
                }
            }

            return View(
                pessoasJuridicas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id is null)     // Se não encontrar os Dados do Colaborador ou é porque não existe o registro ou é porque está logado como UserMain.
                return RedirectToAction(
                    "Index",
                    "Home");    // Em não existindo o registro, redireciona para a página inicial.

            var pessoaJuridica = default(PessoaJuridicaViewModel);

            var tokenBearer = await this._authService.GetTokenAsync();

            string requestUri = @$"PessoaJuridica/{id}";

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                    {
                        var data = JsonConvert.DeserializeObject<PessoaJuridicaResponse>(
                            responseBody);

                        pessoaJuridica = this._mapper.Map<PessoaJuridicaViewModel>(
                            data);
                    }
                }
            }

            return View("Details",
                pessoaJuridica);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return View(
                    new PessoaJuridicaViewModel());

            var pessoaJuridica = default(PessoaJuridicaViewModel);

            var tokenBearer = await this._authService.GetTokenAsync();

            string requestUri = @$"PessoaJuridica/{id}";

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                    {
                        var data = JsonConvert.DeserializeObject<PessoaJuridicaResponse>(
                            responseBody);

                        pessoaJuridica = this._mapper.Map<PessoaJuridicaViewModel>(
                            data);
                    }
                }
            }

            return View("Edit",
                pessoaJuridica);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> Edit(PessoaJuridicaViewModel vm)
        {
            ViewBag.ErrorMessage = null;
            ViewBag.SuccessMessage = null;
            ViewBag.ValidateMessage = null;

            if (!ModelState.IsValid)
                return View(
                    vm);

            bool isNew = vm.Guid is null || vm.Guid == Guid.Empty;

            string cnpjSanitized = vm.Cnpj.Replace(
                ".",
                string.Empty).Replace(
                    "-",
                    string.Empty).Replace(
                        "/",
                        string.Empty);

            string cepSanitized = !string.IsNullOrEmpty(
                vm.Cep) ?
                    vm.Cep.Replace(
                        "-",
                        string.Empty) :
                    string.Empty;

            object request;

            if (isNew)
            {
                var createRequest = this._mapper.Map<PessoaJuridicaCreateRequest>(
                    vm);

                createRequest.Cnpj = cnpjSanitized;

                createRequest.Pessoa = new PessoaCreateRequest()
                {
                    Bairro = vm.Bairro,
                    Cep = cepSanitized,
                    Cidade = vm.Cidade,
                    Complemento = vm.Complemento,
                    Email = vm.Email,
                    Endereco = vm.Endereco,
                    Numero = vm.Numero,
                    Telefone = vm.Telefone,
                    Uf = vm.Uf,
                };

                request = createRequest;
            }
            else
            {
                var updateRequest = this._mapper.Map<PessoaJuridicaUpdateRequest>(
                    vm);

                updateRequest.Cnpj = cnpjSanitized;

                updateRequest.Pessoa = new PessoaUpdateRequest()
                {
                    Bairro = vm.Bairro,
                    Cep = cepSanitized,
                    Cidade = vm.Cidade,
                    Complemento = vm.Complemento,
                    Email = vm.Email,
                    Endereco = vm.Endereco,
                    Numero = vm.Numero,
                    Telefone = vm.Telefone,
                    Uf = vm.Uf,
                };

                request = updateRequest;
            }

            string requestBody = JsonConvert.SerializeObject(
                request,
                Formatting.Indented);

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = isNew ?
                "PessoaJuridica" :
                $"PessoaJuridica/{vm.Guid}";

            var method = isNew ?
                HttpMethod.Post :
                HttpMethod.Put;

            var pessoaJuridicaResponse = default(PessoaJuridicaResponse);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                method,
                requestUri,
                requestBody))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                        pessoaJuridicaResponse = JsonConvert.DeserializeObject<PessoaJuridicaResponse>(
                            responseBody);

                    ViewBag.SuccessMessage = "<p>Aguarde, você será redirecionado em alguns segundos.</p>";
                }
                else
                {
                    string message = "Erro desconhecido ao salvar.";

                    if (responseBody.IsValidJson())
                    {
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(
                            responseBody);

                        message = problemDetails?.Detail ??
                            problemDetails?.Title ??
                            "Erro ao salvar.";
                    }

                    ViewBag.ErrorMessage = $"<p>{message}</p>";
                }
            }

            return View(
                vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> GetDataTable()
        {
            var pessoasJuridicas = default(IEnumerable<PessoaJuridicaViewModel>);

            var tokenBearer = await this._authService.GetTokenAsync();

            //  Inicia o HttpClientSingleton de consumo da API.
            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = "PessoaJuridica";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if (responseBody.IsValidJson())
                    {
                        var source = JsonConvert.DeserializeObject<IEnumerable<PessoaJuridicaResponse>>(
                            responseBody);

                        pessoasJuridicas = this._mapper.Map<IEnumerable<PessoaJuridicaViewModel>>(
                            source);
                    }
                }
            }

            //  Retrieve data from WebApi" 
            var query = from pessoaJuridica in pessoasJuridicas
                        select new
                        {
                            pessoaJuridica.Guid,
                            Cnpj = Convert.ToInt64(
                                pessoaJuridica.Cnpj).ToString(@"00\.000\.000\/0000\-00"),
                            UnidadeNegocio = pessoaJuridica.DescricaoUnidadeNegocio,
                            pessoaJuridica.RazaoSocial,
                            DataFundacao = pessoaJuridica.DataFundacao.HasValue ?
                                pessoaJuridica.DataFundacao.Value.ToString("dd/MM/yyyy") :
                                "__/__/____",
                        };

            var draw = Request.Form["draw"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();

            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();

            var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? string.Empty;
            var start = Request.Form["start"].FirstOrDefault();

            //  Paging Size (10, 20, 50, 100)
            int pageSize = length != null ?
                Convert.ToInt32(
                    length) :
                    0;

            int skip = start != null ?
                Convert.ToInt32(
                    start) :
                    0;

            //  Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                if (!string.IsNullOrEmpty(sortColumnDir) &&
                    sortColumnDir.ToUpper() == "DESC")
                    query = query.OrderByDescending(pf => pf.GetType().GetProperty(
                        sortColumn).GetValue(
                            pf,
                            null));
                else
                    query = query.OrderBy(pf => pf.GetType().GetProperty(
                        sortColumn).GetValue(
                            pf,
                            null));
            }

            //  Search
            if (!string.IsNullOrEmpty(searchValue))
                query = query.Where(
                    td => td.RazaoSocial.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.Cnpj.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.DataFundacao.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.UnidadeNegocio.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        string.IsNullOrEmpty(searchValue));

            //  Total Number of Rows Count.
            int recordsTotal = query.Count();

            //  Paging.
            var data = query.Skip(skip).Take(pageSize).ToList();

            // Create a JSON response with the data and total count.
            return new JsonResult(new
            {
                data,
                draw,
                recordsTotal,
                recordsFiltered = recordsTotal,
            });
        }
    }
}