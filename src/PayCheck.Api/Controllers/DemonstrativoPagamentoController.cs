namespace PayCheck.Api.Controllers
{
    using System;
    using System.Net;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DemonstrativoPagamentoController : ControllerBase
    {
        private readonly IMatriculaDemonstrativoPagamentoService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DemonstrativoPagamentoController(IMatriculaDemonstrativoPagamentoService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet("{guid}")]
        public ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto> GetDemonstrativoPagamento(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data != null)
                    return new ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto>
                {
                    Message = $"Demonstrativo de Pagamento {guid} não encontrado!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto>
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
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetDemonstrativosPagamento()
        {
            try
            {
                var data = this._service.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = $"Demonstrativos de Pagamento não encontrados!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
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
        [HttpGet("Colaborador/{guidColaborador}")]
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetDemonstrativoPagamentoByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                var data = this._service.GetByGuidColaborador(
                    guidColaborador);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = $"Demonstrativos de Pagamento não encontrados para o Colaborador {guidColaborador}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="competencia"></param>
        /// <param name="matricula"></param>
        /// <returns></returns>
        [HttpGet("{competencia}/{matricula}")]
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetDemonstrativoPagamentoByCompetenciaAndMatricula(string competencia, string matricula)
        {
            try
            {
                var data = this._service.Get(
                    competencia,
                    matricula);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = $"Demonstrativos de Pagamento não encontrados para a Competência {competencia} e Matrícula {matricula}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodoInicial"></param>
        /// <param name="periodoFinal"></param>
        /// <returns></returns>
        [HttpGet("getPendencias/{periodoInicial}/{periodoFinal}")]
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetPendencias(string periodoInicial, string periodoFinal)
        {
            try
            {
                var data = this._service.GetPendencias(
                    Convert.ToDateTime(
                        periodoInicial),
                    Convert.ToDateTime(
                        periodoFinal));

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
                {
                    Message = $"Pendências de Demonstrativos de Pagamento não encontrados para o Período de {periodoInicial} a {periodoFinal}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>>
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
        public ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto> UpdateDemonstrativoPagamento(Guid guid, [FromBody] MatriculaDemonstrativoPagamentoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var data = this._service.SaveData(
                    updateDto: updateDto);

                return new ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}