namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using PayCheck.Api.Models;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PessoaJuridicaController : ControllerBase
    {
        private readonly IPessoaJuridicaBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaJuridicaController(IPessoaJuridicaBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete("{guid}")]
        public ApiResponse<PessoaJuridicaResponseDto> DeletePessoaJuridica(Guid guid)
        {
            try
            {
                var apiResponse = this.GetPessoaJuridica(
                    guid);

                this._business.Delete(
                    guid);

                return new ApiResponse<PessoaJuridicaResponseDto>
                {
                    Data = apiResponse.Data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaJuridicaResponseDto>
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
        public ApiResponse<PessoaJuridicaResponseDto> GetPessoaJuridica(Guid guid)
        {
            try
            {
                var data = this._business.Get(
                    guid);

                if (data != null)
                    return new ApiResponse<PessoaJuridicaResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponse<PessoaJuridicaResponseDto>
                {
                    Message = $"Pessoa Jurídica {guid} não encontrada.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaJuridicaResponseDto>
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
        public ApiResponse<IEnumerable<PessoaJuridicaResponseDto>> GetPessoasJuridicas()
        {
            try
            {
                var data = this._business.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponse<IEnumerable<PessoaJuridicaResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponse<IEnumerable<PessoaJuridicaResponseDto>>
                {
                    Message = "Não há registros de Pessoas Juridicas.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<PessoaJuridicaResponseDto>>
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
        public ApiResponse<PessoaJuridicaResponseDto> InsertPessoaJuridica([FromBody] PessoaJuridicaRequestCreateDto createDto)
        {
            try
            {
                var data = this._business.SaveData(
                    createDto);

                return new ApiResponse<PessoaJuridicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.Created,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaJuridicaResponseDto>
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
        public ApiResponse<PessoaJuridicaResponseDto> UpdatePessoaJuridica([FromBody] PessoaJuridicaRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = (Guid)updateDto.Guid;

                var data = this._business.SaveData(
                    updateDto: updateDto);

                return new ApiResponse<PessoaJuridicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaJuridicaResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}