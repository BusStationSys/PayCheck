namespace PayCheck.Web.Controllers
{
    using System;
    using System.Text;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using PayCheck.Web.Models;

    [Authorize]
    public class ColaboradorController : Controller
    {
        private readonly string _tokenBearer;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColaboradorController"/> class.
        /// </summary>
        /// <param name="externalApis"></param>
        public ColaboradorController(IOptions<ExternalApis> externalApis)
        {
            var externalApisValue = externalApis.Value;

            Uri baseAddress = new(
                externalApisValue.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaResponseDto>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaResponseDto>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestCreateDto, PessoaFisicaViewModel>().ReverseMap();
                cfg.CreateMap<PessoaFisicaRequestUpdateDto, PessoaFisicaViewModel>().ReverseMap();

                cfg.CreateMap<PessoaFisicaResponseDto, PessoaFisicaViewModel>().ReverseMap();
            });

            this._mapper = new Mapper(
                mapperConfiguration);

            this._httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };

            using (var webApiHelper = new WebApiHelper(
                string.Concat(
                    baseAddress,
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(Guid? id)
        {
            if (id == null)     // Se não encontrar os Dados do Colaborador ou é porque não existe o registro ou é porque está logado como UserMain.
                return RedirectToAction(
                    "Index",
                    "Home");    // Em não existindo o registro, redireciona para a página inicial.

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{id}";

            var pessoaFisicaViewModel = default(PessoaFisicaViewModel);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(
                        dataJson).Data;

                    pessoaFisicaViewModel = this._mapper.Map<PessoaFisicaViewModel>(
                        data);

                    pessoaFisicaViewModel.Bairro = data.Pessoa.Bairro;
                    pessoaFisicaViewModel.Cep = data.Pessoa.Cep;
                    pessoaFisicaViewModel.Cidade = data.Pessoa.Cidade;
                    pessoaFisicaViewModel.Complemento = data.Pessoa.Complemento;
                    pessoaFisicaViewModel.Email = data.Pessoa.Email;
                    pessoaFisicaViewModel.Endereco = data.Pessoa.Endereco;
                    pessoaFisicaViewModel.Numero = data.Pessoa.Numero;
                    pessoaFisicaViewModel.Telefone = data.Pessoa.Telefone;
                    pessoaFisicaViewModel.Uf = data.Pessoa.Uf;
                }
            }

            return View("Details",
                pessoaFisicaViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Edit(Guid? id)
        {
            if (id == null)
                return View(
                    new PessoaFisicaViewModel());

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{id}";

            var pessoaFisicaViewModel = default(PessoaFisicaViewModel);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(
                        dataJson).Data;

                    pessoaFisicaViewModel = this._mapper.Map<PessoaFisicaViewModel>(
                        data);

                    pessoaFisicaViewModel.Bairro = data.Pessoa.Bairro;
                    pessoaFisicaViewModel.Cep = data.Pessoa.Cep;
                    pessoaFisicaViewModel.Cidade = data.Pessoa.Cidade;
                    pessoaFisicaViewModel.Complemento = data.Pessoa.Complemento;
                    pessoaFisicaViewModel.Email = data.Pessoa.Email;
                    pessoaFisicaViewModel.Endereco = data.Pessoa.Endereco;
                    pessoaFisicaViewModel.Numero = data.Pessoa.Numero;
                    pessoaFisicaViewModel.Telefone = data.Pessoa.Telefone;
                    pessoaFisicaViewModel.Uf = data.Pessoa.Uf;
                }
            }

            return View("Edit",
                pessoaFisicaViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Edit(PessoaFisicaViewModel vm)
        {
            ViewBag.ErrorMessage = null;
            ViewBag.SuccessMessage = null;
            ViewBag.ValidateMessage = null;

            if (!ModelState.IsValid)
            {
                var errorMessageHtml = new StringBuilder();

                var modelStateErrors = this.ModelState.Keys.OrderBy(x => x).SelectMany(key => this.ModelState[key].Errors);

                if (modelStateErrors != null &&
                    modelStateErrors.Count() > 0)
                {
                    errorMessageHtml.Append("<p></p>");

                    foreach (var modelStateError in modelStateErrors)
                    {
                        errorMessageHtml.Append("<p style=\"text-align:justify\">");

                        errorMessageHtml.Append($"- {modelStateError.ErrorMessage}");

                        errorMessageHtml.Append("</p>");
                    }

                    //errorMessageHtml.Append("<ul class=\"list-group\">");

                    //foreach (var modelStateError in modelStateErrors)
                    //{
                    //    errorMessageHtml.Append("<li class=\"list-group-item list-group-item-warning\" style=\"border: none\">");

                    //    errorMessageHtml.Append($"- {modelStateError.ErrorMessage}");

                    //    errorMessageHtml.Append("</li>");
                    //}

                    //errorMessageHtml.Append("</ul>");
                }

                ViewBag.ValidateMessage = errorMessageHtml.ToString();

                return View(
                    vm);
            }

            bool isNew = false;

            var createDto = default(PessoaFisicaRequestCreateDto);
            var updateDto = default(PessoaFisicaRequestUpdateDto);

            if (vm.Guid is null ||
                vm.Guid == Guid.Empty)
            {
                isNew = true;

                createDto = this._mapper.Map<PessoaFisicaRequestCreateDto>(
                    vm);

                createDto.Cpf = createDto.Cpf.Replace(
                    ".",
                    "").Replace(
                        "-",
                        string.Empty);

                createDto.Pessoa = new PessoaRequestCreateDto()
                {
                    Bairro = vm.Bairro,

                    Cep = vm.Cep.Replace(
                        "-",
                        string.Empty),

                    Cidade = vm.Cidade,
                    Complemento = vm.Complemento,
                    Email = vm.Email,
                    Endereco = vm.Endereco,
                    Numero = vm.Numero,
                    Telefone = vm.Telefone,
                    Uf = vm.Uf,
                };
            }
            else
            {
                updateDto = this._mapper.Map<PessoaFisicaRequestUpdateDto>(
                    vm);

                updateDto.Cpf = updateDto.Cpf.Replace(
                    ".",
                    string.Empty).Replace(
                        "-",
                        string.Empty);

                if (updateDto.Pessoa is null)
                    updateDto.Pessoa = new PessoaRequestUpdateDto();

                updateDto.Pessoa.Bairro = vm.Bairro;

                updateDto.Pessoa.Cep = !string.IsNullOrEmpty(vm.Cep) ?
                    vm.Cep.Replace(
                        "-",
                        string.Empty) :
                    string.Empty;

                updateDto.Pessoa.Cidade = vm.Cidade;
                updateDto.Pessoa.Complemento = vm.Complemento;
                updateDto.Pessoa.Email = vm.Email;
                updateDto.Pessoa.Endereco = vm.Endereco;
                updateDto.Pessoa.Numero = vm.Numero;
                updateDto.Pessoa.Telefone = vm.Telefone;
                updateDto.Pessoa.Uf = vm.Uf;
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica";

            string fromBodyString = JsonConvert.SerializeObject(
                isNew ?
                    createDto :
                    updateDto,
                Formatting.Indented);

            var apiResponseDto = default(ApiResponseDto<PessoaFisicaResponseDto>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                if (isNew)
                    fromBodyString = webApiHelper.ExecutePostWithAuthenticationByBearer(
                        fromBodyString);
                else
                    fromBodyString = webApiHelper.ExecutePutWithAuthenticationByBearer(
                        fromBodyString);

                if (fromBodyString.IsValidJson())
                    apiResponseDto = JsonConvert.DeserializeObject<ApiResponseDto<PessoaFisicaResponseDto>>(
                        fromBodyString);
            }

            if (apiResponseDto != null &&
                apiResponseDto.Success)
                ViewBag.SuccessMessage = "<p>Aguarde, você será redirecionado em alguns segundos.</p>";
            else
                ViewBag.ErrorMessage = $"<p>{apiResponseDto.Message}</p>";

            return View(
                vm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult GetDataTable()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica";

            var pessoasFisicas = default(IEnumerable<PessoaFisicaViewModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var source = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>>(
                        dataJson).Data;

                    pessoasFisicas = this._mapper.Map<IEnumerable<PessoaFisicaViewModel>>(
                        source);
                }
            }

            //  Retrieve data from WebApi
            var query = from pessoaFisica in pessoasFisicas
                        select new
                        {
                            pessoaFisica.Guid,
                            Cpf = Convert.ToInt64(
                                pessoaFisica.Cpf).ToString(@"000\.000\.000\-00"),
                            DataNascimento = pessoaFisica.DataNascimento.HasValue ?
                                pessoaFisica.DataNascimento.Value.ToString("dd/MM/yyyy") :
                                "__/__/____",
                            pessoaFisica.Nome,
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
                    td => td.Nome.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.Cpf.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                        td.DataNascimento.Contains(
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