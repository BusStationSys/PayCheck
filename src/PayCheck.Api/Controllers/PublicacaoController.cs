namespace PayCheck.Api.Controllers
{
    using System.Net;
    using ARVTech.DataAccess.Business.UniPayCheck.Interfaces;
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PublicacaoController : ControllerBase
    {
        private readonly IPublicacaoBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PublicacaoController(IPublicacaoBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ApiResponseDto<PublicacaoResponseDto> DeletePublicacao(int id)
        {
            try
            {
                var apiResponse = this.GetPublicacao(
                    id);

                this._business.Delete(
                    id);

                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Data = apiResponse.Data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ApiResponseDto<PublicacaoResponseDto> GetPublicacao(int id)
        {
            try
            {
                var data = this._business.Get(
                    id);

                if (data != null)
                    return new ApiResponseDto<PublicacaoResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Message = $"Publicação {id} não encontrada.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PublicacaoResponseDto>
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
        [HttpGet()]
        public ApiResponseDto<IEnumerable<PublicacaoResponseDto>> GetPublicacoes()
        {
            try
            {
                var data = this._business.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
                {
                    Message = "Não há registros de Publicações.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("getImage/{id}")]
        public ApiResponseDto<PublicacaoResponseDto> GetImage(int id)
        {
            try
            {
                var data = this._business.GetImage(
                    id);

                if (data != null)
                    return new ApiResponseDto<PublicacaoResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Message = $"Não há registro Imagem do Id {id}.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAtualString"></param>
        /// <returns></returns>
        [HttpGet("getSobreNos/{dataAtualString}")]
        public ApiResponseDto<IEnumerable<PublicacaoResponseDto>> GetSobreNos(string dataAtualString)
        {
            try
            {
                var data = this._business.GetSobreNos(
                    dataAtualString);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
                {
                    Message = $"Não há registros *Sobre Nós* para serem exibidos em {dataAtualString}.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<PublicacaoResponseDto>>
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
        public ApiResponseDto<PublicacaoResponseDto> CreatePublicacao([FromBody] PublicacaoRequestCreateDto createDto)
        {
            try
            {
                var data = this._business.SaveData(
                    createDto);

                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.Created,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PublicacaoResponseDto>
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
        public ApiResponseDto<PublicacaoResponseDto> UpdatePublicacao([FromBody] PublicacaoRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Id = (int)updateDto.Id;

                var data = this._business.SaveData(
                    updateDto: updateDto);

                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<PublicacaoResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}