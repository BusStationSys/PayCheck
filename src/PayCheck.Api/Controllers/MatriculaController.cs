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
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MatriculaController(IMatriculaService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="guid"></param>
        ///// <returns></returns>
        //[HttpDelete("{guid}")]
        //public ApiResponseDto<PessoaFisicaResponseDto> DeletePessoaFisica(Guid guid)
        //{
        //    try
        //    {
        //        var apiResponse = this.GetPessoaFisica(
        //            guid);

        //        this._business.Delete(
        //            guid);

        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Data = apiResponse.Data,
        //            Success = true,
        //            StatusCode = HttpStatusCode.NoContent,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Message = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        };
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="guid"></param>
        ///// <returns></returns>
        //[HttpGet("{guid}")]
        //public ApiResponseDto<PessoaFisicaResponseDto> GetPessoaFisica(Guid guid)
        //{
        //    try
        //    {
        //        var data = this._business.Get(
        //            guid);

        //        if (data != null)
        //            return new ApiResponseDto<PessoaFisicaResponseDto>
        //            {
        //                Data = data,
        //                Success = true,
        //                StatusCode = HttpStatusCode.OK,
        //            };

        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Message = $"Pessoa Física {guid} não encontrada.",
        //            StatusCode = HttpStatusCode.NotFound,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Message = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        };
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>> GetPessoasFisicas()
        //{
        //    try
        //    {
        //        var data = this._business.GetAll();

        //        if (data != null && data.Count() > 0)
        //            return new ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>
        //            {
        //                Data = data,
        //                Success = true,
        //                StatusCode = HttpStatusCode.OK,
        //            };

        //        return new ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>
        //        {
        //            Message = "Não há registros de Pessoas Físicas.",
        //            StatusCode = HttpStatusCode.NotFound,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponseDto<IEnumerable<PessoaFisicaResponseDto>>
        //        {
        //            Message = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        };
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("getAniversariantesEmpresa/{mes}")]
        public ApiResponseDto<IEnumerable<MatriculaResponseDto>> GetAniversariantesEmpresa(int mes)
        {
            try
            {
                var data = this._service.GetAniversariantesEmpresa(
                    mes);

                if (data != null && data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<MatriculaResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<MatriculaResponseDto>>
                {
                    Message = $"Não há registros de Matrículas que realizam aniversário de empresa em {mes}.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<MatriculaResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="createDto"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ApiResponseDto<PessoaFisicaResponseDto> CreatePessoaFisica([FromBody] PessoaFisicaRequestCreateDto createDto)
        //{
        //    try
        //    {
        //        var data = this._business.SaveData(
        //            createDto);

        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Data = data,
        //            Success = true,
        //            StatusCode = HttpStatusCode.Created,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Message = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        };
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="updateDto"></param>
        ///// <returns></returns>
        //[HttpPut]
        //public ApiResponseDto<PessoaFisicaResponseDto> UpdatePessoaFisica([FromBody] PessoaFisicaRequestUpdateDto updateDto)
        //{
        //    try
        //    {
        //        updateDto.Guid = (Guid)updateDto.Guid;

        //        var data = this._business.SaveData(
        //            updateDto: updateDto);

        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Data = data,
        //            Success = true,
        //            StatusCode = HttpStatusCode.NoContent,
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ApiResponseDto<PessoaFisicaResponseDto>
        //        {
        //            Message = ex.Message,
        //            StatusCode = HttpStatusCode.InternalServerError,
        //        };
        //    }
        //}
    }
}