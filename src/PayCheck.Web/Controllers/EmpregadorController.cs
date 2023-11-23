namespace PayCheck.Web.Controllers
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.Shared.Extensions;
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using PayCheck.Web.Models;

    public class EmpregadorController : Controller
    {
        private readonly string _tokenBearer;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpregadorController"/> class.
        /// </summary>
        /// <param name="externalApis"></param>
        public EmpregadorController(IOptions<ExternalApis> externalApis)
        {
            var externalApisValue = externalApis.Value;

            Uri baseAddress = new(
                externalApisValue.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaResponseDto>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaResponseDto>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestCreateDto, PessoaJuridicaViewModel>().ReverseMap();
                cfg.CreateMap<PessoaJuridicaRequestUpdateDto, PessoaJuridicaViewModel>().ReverseMap();

                cfg.CreateMap<PessoaJuridicaResponseDto, PessoaJuridicaViewModel>().ReverseMap();
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
            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaJuridica";

            var pessoasJuridicasViewModel = default(IEnumerable<PessoaJuridicaViewModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PessoaJuridicaResponseDto>>>(
                        dataJson).Data;

                    pessoasJuridicasViewModel = this._mapper.Map<IEnumerable<PessoaJuridicaViewModel>>(
                        data);
                }
            }

            return View(
                pessoasJuridicasViewModel);
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
                    new PessoaJuridicaViewModel());

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaJuridica/{guid}";

            var pessoaJuridicaViewModel = default(PessoaJuridicaViewModel);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<PessoaJuridicaResponseDto>>(
                        dataJson).Data;

                    pessoaJuridicaViewModel = this._mapper.Map<PessoaJuridicaViewModel>(
                        data);

                    pessoaJuridicaViewModel.Bairro = data.Pessoa.Bairro;
                    pessoaJuridicaViewModel.Cep = data.Pessoa.Cep;
                    pessoaJuridicaViewModel.Cidade = data.Pessoa.Cidade;
                    pessoaJuridicaViewModel.Complemento = data.Pessoa.Complemento;
                    pessoaJuridicaViewModel.Email = data.Pessoa.Email;
                    pessoaJuridicaViewModel.Endereco = data.Pessoa.Endereco;
                    pessoaJuridicaViewModel.Numero = data.Pessoa.Numero;
                    pessoaJuridicaViewModel.Telefone = data.Pessoa.Telefone;
                    pessoaJuridicaViewModel.Uf = data.Pessoa.Uf;
                }
            }

            return View("Edit",
                pessoaJuridicaViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Edit(PessoaJuridicaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            bool isNew = false;

            var createDto = default(PessoaJuridicaRequestCreateDto);
            var updateDto = default(PessoaJuridicaRequestUpdateDto);

            if (vm.Guid is null ||
                vm.Guid == Guid.Empty)
            {
                isNew = true;

                createDto = this._mapper.Map<PessoaJuridicaRequestCreateDto>(
                    vm);

                createDto.Cnpj = createDto.Cnpj.Replace(
                    ".",
                    "").Replace(
                        "/",
                        "").Replace(
                            "-",
                            "");

                createDto.Pessoa = new PessoaRequestCreateDto()
                {
                    Bairro = vm.Bairro,

                    Cep = !string.IsNullOrEmpty(vm.Cep) ?
                        vm.Cep.Replace(
                            "-",
                            "") :
                        string.Empty,

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
                updateDto = this._mapper.Map<PessoaJuridicaRequestUpdateDto>(
                    vm);

                updateDto.Cnpj = updateDto.Cnpj.Replace(
                    ".",
                    "").Replace(
                        "/",
                        "").Replace(
                            "-",
                            "");

                if (updateDto.Pessoa is null)
                    updateDto.Pessoa = new PessoaRequestUpdateDto();

                updateDto.Pessoa.Bairro = vm.Bairro;

                updateDto.Pessoa.Cep = !string.IsNullOrEmpty(vm.Cep) ?
                    vm.Cep.Replace(
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

            string requestUri = @$"{this._httpClient.BaseAddress}/PessoaJuridica";

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