namespace PayCheck.Web.Controllers
{
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
        [HttpGet()]
        public IActionResult Index()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica";

            var pessoasFisicasViewModel = default(IEnumerable<PessoaFisicaViewModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>>(
                        dataJson).Data;

                    pessoasFisicasViewModel = this._mapper.Map<IEnumerable<PessoaFisicaViewModel>>(
                        data);
                }
            }

            return View(
                pessoasFisicasViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Edit(Guid? guid)
        {
            if (guid == null)
                return View(
                    new PessoaFisicaViewModel());

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaFisica/{guid}";

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
            if (!ModelState.IsValid)
                return View(vm);

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
                        "");

                createDto.Pessoa = new PessoaRequestCreateDto()
                {
                    Bairro = vm.Bairro,

                    Cep = vm.Cep.Replace(
                        "-",
                        ""),

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
                    "").Replace(
                        "-",
                        "");

                if (updateDto.Pessoa is null)
                    updateDto.Pessoa = new PessoaRequestUpdateDto();

                updateDto.Pessoa.Bairro = vm.Bairro;

                updateDto.Pessoa.Cep = !string.IsNullOrEmpty(vm.Cep) ? vm.Cep.Replace(
                    "-",
                    "") :
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
                {

                }
            }

            return RedirectToAction(
                "Index");
        }
    }
}