namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EspelhoPontoController : ControllerBase
    {
        private readonly IMatriculaEspelhoPontoService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EspelhoPontoController(IMatriculaEspelhoPontoService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));

            //var mapperConfiguration = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
            //    cfg.CreateMap<MatriculaEspelhoPontoRequestCreateDto, MatriculaEspelhoPontoResponse>().ReverseMap();
            //});

            //this._mapper = new Mapper(
            //    mapperConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet("{guid}")]
        public ApiResponseDto<MatriculaEspelhoPontoResponseDto> GetEspelhoPonto(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data != null)
                    return new ApiResponseDto<MatriculaEspelhoPontoResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<MatriculaEspelhoPontoResponseDto>
                {
                    Message = $"Espelho de Ponto {guid} não encontrado!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<MatriculaEspelhoPontoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>> GetEspelhosPonto()
        {
            try
            {
                var data = this._service.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                {
                    Message = "Não há registros de Espelhos de Ponto.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guidColaborador"></param>
        /// <returns></returns>
        [HttpGet("getEspelhoPontoByGuidColaborador/{guidColaborador}")]
        public ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>> GetEspelhoPontoByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                var data = this._service.GetByGuidColaborador(
                    guidColaborador);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                {
                    Message = $"Espelhos de Ponto não encontrados para o Colaborador {guidColaborador}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaEspelhoPontoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPut("{guid}")]
        public ApiResponseDto<MatriculaEspelhoPontoResponseDto> UpdateMatriculaEspelhoPonto(Guid guid, [FromBody] MatriculaEspelhoPontoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var data = this._service.SaveData(
                    updateDto: updateDto);

                return new ApiResponseDto<MatriculaEspelhoPontoResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<MatriculaEspelhoPontoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}