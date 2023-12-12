namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EspelhoPontoController : ControllerBase
    {
        private readonly IMatriculaEspelhoPontoBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public EspelhoPontoController(IMatriculaEspelhoPontoBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));

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
                var data = this._business.Get(
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
                var data = this._business.GetAll();

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
                var data = this._business.GetByGuidColaborador(
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

                var data = this._business.SaveData(
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