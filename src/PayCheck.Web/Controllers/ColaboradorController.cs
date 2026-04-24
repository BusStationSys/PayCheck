namespace PayCheck.Web.Controllers
{
    using System;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Models;
    using PayCheck.Web.Services.Interfaces;

    [Authorize]
    public class ColaboradorController : Controller
    {
        private readonly IHttpClientService _httpClientService;

        private readonly IAuthService _authService;

        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColaboradorController"/> class.
        /// </summary>
        /// <param name="httpClientService">The HTTP client service.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public ColaboradorController(IHttpClientService httpClientService, IAuthService authService, IMapper mapper)
        {
            this._httpClientService = httpClientService;

            this._authService = authService;

            this._mapper = mapper;
        }

        /// <summary>
        /// Returns the default view for the pessoa física page.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the corresponding view.
        /// </returns>
        [HttpGet()]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Displays the details view for a specific "Pessoa Física".
        /// </summary>
        /// <remarks>
        /// If the identifier is not provided, the user is redirected to the Home/Index page.
        /// Otherwise, the pessoa física data is loaded and passed to the details view.rvice httpClientService, IAuthService authService, IMapper mapper)
        /// </remarks>
        /// <param name="id">The unique identifier of the pessoa física.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the details view or redirects when the identifier is null.
        /// </returns>
        [HttpGet()]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id is null)
                return RedirectToAction(
                    "Index",
                    "Home");

            var pessoaFisicaViewModel = await this.LoadPessoaFisicaAsync(
                id.Value);

            return View(
                "Details",
                pessoaFisicaViewModel);
        }

        /// <summary>
        /// Displays the edit view for a "Pessoa Física".
        /// </summary>
        /// <remarks>
        /// If the identifier is null, a new instance is created and passed to the view,
        /// allowing the creation of a new record. Otherwise, the existing "Pessoa Física"
        /// data is loaded and provided for editing.
        /// </remarks>
        /// <param name="id">The unique identifier of the "Pessoa Física".</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the edit view with either a new or existing model.
        /// </returns>
        [HttpGet()]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id is null)
                return View(
                    new PessoaFisicaModel());

            var pessoaFisicaViewModel = await this.LoadPessoaFisicaAsync(
                id.Value);

            return View(
                "Edit",
                pessoaFisicaViewModel);
        }

        /// <summary>
        /// Handles the submission of "Pessoa Física" data for creation or update.
        /// </summary>
        /// <remarks>
        /// Validates the incoming model and determines whether the operation is a create or update based on the identifier.
        /// The data is sanitized, mapped to the appropriate request DTO, and sent to an external API using an authenticated request.
        /// 
        /// The API response is validated and deserialized, and success or error messages are assigned to the view accordingly.
        /// In case of validation failure, the original view model is returned without processing.
        /// </remarks>
        /// <param name="vm">The view model containing "Pessoa Física" data.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> that returns the view with success or error feedback.
        /// </returns>
        [HttpPost()]
        public async Task<IActionResult> Edit(PessoaFisicaModel vm)
        {
            ViewBag.ErrorMessage = null;
            ViewBag.SuccessMessage = null;

            if (!ModelState.IsValid)
                return View(vm);

            bool isNew = vm.Guid is null || vm.Guid == Guid.Empty;

            string cpfSanitized = vm.Cpf.Replace(
                ".",
                string.Empty).Replace(
                    "-",
                    string.Empty);

            string cepSanitized = !string.IsNullOrEmpty(
                vm.Cep) ?
                    vm.Cep.Replace(
                        "-",
                        string.Empty) :
                    string.Empty;

            object dto;

            if (isNew)
            {
                var createDto = this._mapper.Map<PessoaFisicaRequestCreateDto>(
                    vm);

                createDto.Cpf = cpfSanitized;

                createDto.Pessoa = new PessoaRequestCreateDto()
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

                dto = createDto;
            }
            else
            {
                var updateDto = this._mapper.Map<PessoaFisicaRequestUpdateDto>(
                    vm);

                updateDto.Cpf = cpfSanitized;

                updateDto.Pessoa = new PessoaRequestUpdateDto()
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

                dto = updateDto;
            }

            string requestBody = JsonConvert.SerializeObject(
                dto,
                Formatting.Indented);

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = isNew ?
                "PessoaFisica" :
                $"PessoaFisica/{vm.Guid}";

            var method = isNew ?
                HttpMethod.Post :
                HttpMethod.Put;

            var apiResponseDto = default(ApiResponseDto<PessoaFisicaResponseDto>);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                method,
                requestUri,
                requestBody))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string responseBody = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (responseBody.IsValidJson())
                        apiResponseDto = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(
                            responseBody);
                }
            }

            if (apiResponseDto?.Success is true)
                ViewBag.SuccessMessage = "<p>Aguarde, você será redirecionado em alguns segundos.</p>";
            else
                ViewBag.ErrorMessage = $"<p>{apiResponseDto?.Message ?? "Erro desconhecido ao salvar."}</p>";

            return View(
                vm);
        }

        /// <summary>
        /// Retrieves and returns a paginated, filtered, and sorted list of pessoa física records formatted for DataTables consumption.
        /// </summary>
        /// <remarks>
        /// This method performs an authenticated HTTP GET request to an external API to fetch pessoa física data,
        /// validates and deserializes the response, and maps it to domain models.
        /// The result is then transformed into a simplified anonymous object, including formatted CPF and birth date.
        /// 
        /// It also applies server-side processing based on DataTables parameters received via the request,
        /// including paging, sorting, and filtering.
        /// 
        /// Returns a JSON result compatible with DataTables, containing the data set and metadata such as
        /// total records, filtered records, and draw counter.
        /// </remarks>
        /// <returns>
        /// A <see cref="JsonResult"/> containing the processed data and DataTables metadata.
        /// </returns>
        [HttpPost()]
        public async Task<IActionResult> GetDataTable()
        {
            var pessoasFisicas = Enumerable.Empty<PessoaFisicaModel>();

            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(
                tokenBearer);

            string requestUri = "PessoaFisica";

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                requestUri))
            {
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                    if (dataJson.IsValidJson())
                    {
                        var source = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>>(
                            dataJson).Data;

                        pessoasFisicas = this._mapper.Map<IEnumerable<PessoaFisicaModel>>(
                            source);
                    }
                }
            }

            var query = from pessoaFisica in pessoasFisicas
                        let cpfNumerico = pessoaFisica.Cpf?
                            .Replace(".", string.Empty)
                            .Replace("-", string.Empty)
                        let cpfFormatado = long.TryParse(cpfNumerico, out var cpfLong)
                            ? cpfLong.ToString(@"000\.000\.000\-00")
                            : pessoaFisica.Cpf ?? string.Empty
                        select new
                        {
                            pessoaFisica.Guid,
                            Cpf = cpfFormatado,
                            DataNascimento = pessoaFisica.DataNascimento.HasValue
                                ? pessoaFisica.DataNascimento.Value.ToString("dd/MM/yyyy")
                                : "__/__/____",
                            pessoaFisica.Nome,
                        };

            var draw = Request.Form["draw"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? string.Empty;
            var start = Request.Form["start"].FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = !string.IsNullOrEmpty(sortColumnDir) && sortColumnDir.Equals("DESC", StringComparison.OrdinalIgnoreCase)
                    ? query.OrderByDescending(pf => pf.GetType().GetProperty(sortColumn).GetValue(pf, null))
                    : query.OrderBy(pf => pf.GetType().GetProperty(sortColumn).GetValue(pf, null));
            }

            if (!string.IsNullOrEmpty(searchValue))
                query = query.Where(td =>
                    td.Nome.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    td.Cpf.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                    td.DataNascimento.Contains(searchValue, StringComparison.OrdinalIgnoreCase));

            //  int recordsTotal = query.Count();
            int recordsTotal = pessoasFisicas.Count();  // Total antes do filtro.

            int recordsFiltered = query.Count();        // Total após o filtro.

            var data = query.Skip(skip).Take(pageSize).ToList();

            return new JsonResult(
                new
                {
                    data,
                    draw,
                    recordsTotal,
                    recordsFiltered,
                });
        }

        /// <summary>
        /// Retrieves a <see cref="PessoaFisicaModel"/> from the API based on the specified identifier.
        /// </summary>
        /// <remarks>
        /// This method performs an authenticated HTTP GET request to obtain the data of a pessoa física,
        /// validates the JSON response, deserializes it into a DTO, and maps it to the domain model.
        /// Additional address-related properties are manually assigned after mapping.
        /// Returns <c>null</c> if the request fails or the response content is invalid.
        /// </remarks>
        /// <param name="id">The unique identifier of the pessoa física.</param>
        /// <returns>
        /// A populated <see cref="PessoaFisicaModel"/> instance if successful; otherwise, <c>null</c>.
        /// </returns>
        private async Task<PessoaFisicaModel?> LoadPessoaFisicaAsync(Guid id)
        {
            var tokenBearer = await this._authService.GetTokenAsync();

            this._httpClientService.SetBearerAuthentication(tokenBearer);

            using var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Get,
                $"PessoaFisica/{id}");
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                    return null;

                string dataJson = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!dataJson.IsValidJson())
                    return null;

                var apiResponse = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(dataJson);

                return apiResponse?.Data is not null
                    ? this._mapper.Map<PessoaFisicaModel>(apiResponse.Data)
                    : null;
            }
        }
    }
}