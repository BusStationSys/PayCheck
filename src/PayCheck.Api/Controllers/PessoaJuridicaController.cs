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
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly IPessoaJuridicaService _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaJuridicaController(IPessoaJuridicaService service)
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
        [HttpDelete("{guid}")]
        public ApiResponseDto<PessoaJuridicaResponseDto> DeletePessoaJuridica(Guid guid)
        {
            try
            {
                var apiResponse = this.GetPessoaJuridica(
                    guid);

                this._service.Delete(
                    guid);

                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Data = apiResponse.Data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PessoaJuridicaResponseDto>
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
        /// <returns></returns>
        [HttpGet("{guid}")]
        public ApiResponseDto<PessoaJuridicaResponseDto> GetPessoaJuridica(Guid guid)
        {
            try
            {
                var data = this._service.Get(
                    guid);

                if (data != null)
                    return new ApiResponseDto<PessoaJuridicaResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Message = $"Pessoa Jurídica {guid} não encontrada.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PessoaJuridicaResponseDto>
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
        public ApiResponseDto<IEnumerable<PessoaJuridicaResponseDto>> GetPessoasJuridicas()
        {
            try
            {
                var data = this._service.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<PessoaJuridicaResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<PessoaJuridicaResponseDto>>
                {
                    Message = "Não há registros de Pessoas Juridicas.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<PessoaJuridicaResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="createDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ApiResponseDto<PessoaJuridicaResponseDto> InsertPessoaJuridica([FromBody] PessoaJuridicaRequestCreateDto createDto)
        {
            try
            {
                var data = this._service.SaveData(
                    createDto);

                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.Created,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPut]
        public ApiResponseDto<PessoaJuridicaResponseDto> UpdatePessoaJuridica([FromBody] PessoaJuridicaRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = (Guid)updateDto.Guid;

                var data = this._service.SaveData(
                    updateDto: updateDto);

                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PessoaJuridicaResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}