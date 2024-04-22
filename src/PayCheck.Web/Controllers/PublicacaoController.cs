namespace PayCheck.Web.Controllers
{
    using System.IO;
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
    public class PublicacaoController : Controller
    {
        private readonly string _tokenBearer;

        private readonly HttpClient _httpClient;

        private readonly Mapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublicacaoController"/> class.
        /// </summary>
        /// <param name="externalApis"></param>
        public PublicacaoController(IOptions<ExternalApis> externalApis)
        {
            var externalApisValue = externalApis.Value;

            Uri baseAddress = new(
                externalApisValue.PayCheck);

            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PublicacaoRequestCreateDto, PublicacaoResponseDto>().ReverseMap();
                cfg.CreateMap<PublicacaoRequestUpdateDto, PublicacaoResponseDto>().ReverseMap();
                cfg.CreateMap<PublicacaoRequestCreateDto, PublicacaoModel>().ReverseMap();
                cfg.CreateMap<PublicacaoRequestUpdateDto, PublicacaoModel>().ReverseMap();

                cfg.CreateMap<PublicacaoResponseDto, PublicacaoModel>().ReverseMap();
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
            string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao";

            var publicacoes = default(IEnumerable<PublicacaoModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PublicacaoResponseDto>>>(
                        dataJson).Data;

                    publicacoes = this._mapper.Map<IEnumerable<PublicacaoModel>>(
                        data);
                }
            }

            return View(
                publicacoes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao/{id}";

            var publicacao = default(PublicacaoModel);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<PublicacaoResponseDto>>(
                        dataJson).Data;

                    publicacao = this._mapper.Map<PublicacaoModel>(
                        data);
                }
            }

            return View("Details",
                publicacao);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet()]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return View(
                    new PublicacaoModel());

            string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao/{id}";

            var publicacao = default(PublicacaoModel);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var data = JsonConvert.DeserializeObject<ApiResponseDto<PublicacaoResponseDto>>(
                        dataJson).Data;

                    publicacao = this._mapper.Map<PublicacaoModel>(
                        data);
                }
            }

            return View("Edit",
                publicacao);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Edit(PublicacaoModel model, List<IFormFile> files, List<IFormFile> images)
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
                }

                ViewBag.ValidateMessage = errorMessageHtml.ToString();

                return View(
                    model);
            }

            //  Faz a validação da Imagem.
            if (string.IsNullOrEmpty(
                model.NomeImagem) || 
                    images?.Count() == 0)
            {
                string validateMessage = this.ValidateUpload(
                    images);

                if (!string.IsNullOrEmpty(validateMessage))
                {
                    var errorMessageHtml = new StringBuilder();

                    errorMessageHtml.Append("<p></p>");

                    errorMessageHtml.Append("<p style=\"text-align:justify\">");

                    errorMessageHtml.Append($@"- {validateMessage}.");

                    errorMessageHtml.Append("</p>");

                    ViewBag.ValidateMessage = errorMessageHtml.ToString();

                    return View(
                        model);
                }
            }

            bool isNew = false;

            var createDto = default(PublicacaoRequestCreateDto);
            var updateDto = default(PublicacaoRequestUpdateDto);

            if (model.Id is null)
            {
                isNew = true;

                if (files?.Count > 0 &&
                    files?.FirstOrDefault().FileName != null)
                {
                    model.ConteudoArquivo = this.GetContentUpload(
                        files?.FirstOrDefault());
                    model.NomeArquivo = files?.FirstOrDefault().FileName;
                }

                model.ConteudoImagem = this.GetContentUpload(
                    images?.FirstOrDefault());
                model.NomeImagem = images?.FirstOrDefault().FileName;

                createDto = this._mapper.Map<PublicacaoRequestCreateDto>(
                    model);

                createDto.ExtensaoImagem = new FileInfo(
                    model.NomeImagem).Extension.Replace(
                        ".",
                        string.Empty);

                if (!string.IsNullOrEmpty(
                    model.NomeArquivo))
                    createDto.ExtensaoArquivo = new FileInfo(
                        model.NomeArquivo).Extension.Replace(
                            ".",
                            string.Empty);
            }
            else
            {
                if (files?.Count > 0)
                {
                    model.ConteudoArquivo = this.GetContentUpload(
                        files?.FirstOrDefault());
                    model.NomeArquivo = files?.FirstOrDefault().FileName;
                }

                if (images?.Count > 0)
                {
                    model.ConteudoImagem = this.GetContentUpload(
                        images?.FirstOrDefault());
                    model.NomeImagem = images?.FirstOrDefault().FileName;
                }

                updateDto = this._mapper.Map<PublicacaoRequestUpdateDto>(
                    model);

                updateDto.ExtensaoImagem = new FileInfo(
                    model.NomeImagem).Extension.Replace(
                        ".",
                        string.Empty);

                if (!string.IsNullOrEmpty(model.NomeArquivo))
                    updateDto.ExtensaoArquivo = new FileInfo(
                        model.NomeArquivo).Extension.Replace(
                            ".",
                            string.Empty);
            }

            string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao";

            string fromBodyString = JsonConvert.SerializeObject(
                isNew ?
                    createDto :
                    updateDto,
                Formatting.Indented);

            var apiResponseDto = default(ApiResponseDto<PublicacaoResponseDto>);

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
                    apiResponseDto = JsonConvert.DeserializeObject<ApiResponseDto<PublicacaoResponseDto>>(
                        fromBodyString);
            }

            if (apiResponseDto != null &&
                apiResponseDto.Success)
                ViewBag.SuccessMessage = "<p>Aguarde, você será redirecionado em alguns segundos.</p>";
            else
                ViewBag.ErrorMessage = $"<p>{apiResponseDto.Message}</p>";

            return View(
                model);
        }

        private byte[] GetContentUpload(IFormFile upload)
        {
            try
            {
                if (upload != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        upload.CopyTo(
                            memoryStream);

                        return memoryStream.ToArray();
                    }
                }

                return Array.Empty<byte>();
            }
            catch
            {
                throw;
            }
        }

        private string ValidateUpload(List<IFormFile> upload)
        {
            if (upload?.Count == 0)
                return "É necessário indicar pelo menos 1 arquivo";

            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult GetDataTable()
        {
            string requestUri = @$"{this._httpClient.BaseAddress}/Publicacao";

            var publicacoes = default(IEnumerable<PublicacaoModel>);

            using (var webApiHelper = new WebApiHelper(
                requestUri,
                this._tokenBearer))
            {
                string dataJson = webApiHelper.ExecuteGetWithAuthenticationByBearer();

                if (dataJson.IsValidJson())
                {
                    var source = JsonConvert.DeserializeObject<ApiResponseDto<IEnumerable<PublicacaoResponseDto>>>(
                        dataJson).Data;

                    publicacoes = this._mapper.Map<IEnumerable<PublicacaoModel>>(
                        source);
                }
            }

            //  Retrieve data from WebApi" 
            var query = from publicacao in publicacoes
                        select new
                        {
                            publicacao.Id,
                            DataApresentacao = publicacao.DataApresentacao.HasValue ?
                                publicacao.DataApresentacao.Value.ToString("dd/MM/yyyy") :
                                "__/__/____",
                            publicacao.Titulo,
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
                    query = query.OrderByDescending(p => p.GetType().GetProperty(
                        sortColumn).GetValue(
                            p,
                            null));
                else
                    query = query.OrderBy(p => p.GetType().GetProperty(
                        sortColumn).GetValue(
                            p,
                            null));
            }

            //  Search
            if (!string.IsNullOrEmpty(searchValue))
                query = query.Where(
                    td => td.Titulo.Contains(
                            searchValue,
                            StringComparison.OrdinalIgnoreCase) ||
                          td.DataApresentacao.Contains(
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