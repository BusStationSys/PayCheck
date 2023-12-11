namespace PayCheck.Api.Controllers
{
    using System;
    using System.Net;
    using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
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
        private readonly IMatriculaDemonstrativoPagamentoBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DemonstrativoPagamentoController(IMatriculaDemonstrativoPagamentoBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
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
                var data = this._business.Get(
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
                var data = this._business.GetAll();

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
        [HttpGet("getDemonstrativoPagamentoByGuidColaborador/{guidColaborador}")]
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetDemonstrativoPagamentoByGuidColaborador(Guid guidColaborador)
        {
            try
            {
                var data = this._business.GetByGuidColaborador(
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
                var data = this._business.Get(
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
        /// <param name="guid"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPut("{guid}")]
        public ApiResponseDto<MatriculaDemonstrativoPagamentoResponseDto> UpdateDemonstrativoPagamento(Guid guid, [FromBody] MatriculaDemonstrativoPagamentoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = guid;

                var data = this._business.SaveData(
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