﻿namespace PayCheck.Api.Controllers
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
    public class PessoaFisicaController : ControllerBase
    {
        private readonly IPessoaFisicaBusiness _business;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="business"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public PessoaFisicaController(IPessoaFisicaBusiness business)
        {
            this._business = business ?? throw new ArgumentNullException(nameof(business));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpDelete("{guid}")]
        public ApiResponse<PessoaFisicaResponseDto> DeletePessoaFisica(Guid guid)
        {
            try
            {
                var apiResponse = this.GetPessoaFisica(
                    guid);

                this._business.Delete(
                    guid);

                return new ApiResponse<PessoaFisicaResponseDto>
                {
                    Data = apiResponse.Data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaFisicaResponseDto>
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
        public ApiResponse<PessoaFisicaResponseDto> GetPessoaFisica(Guid guid)
        {
            try
            {
                var data = this._business.Get(
                    guid);

                if (data != null)
                    return new ApiResponse<PessoaFisicaResponseDto>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponse<PessoaFisicaResponseDto>
                {
                    Message = $"Pessoa Física {guid} não encontrada.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaFisicaResponseDto>
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
        public ApiResponse<IEnumerable<PessoaFisicaResponseDto>> GetPessoasFisicas()
        {
            try
            {
                var data = this._business.GetAll();

                if (data != null && data.Count() > 0)
                    return new ApiResponse<IEnumerable<PessoaFisicaResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponse<IEnumerable<PessoaFisicaResponseDto>>
                {
                    Message = "Não há registros de Pessoas Físicas.",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<PessoaFisicaResponseDto>>
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
        public ApiResponse<PessoaFisicaResponseDto> InsertPessoaFisica([FromBody] PessoaFisicaRequestCreateDto createDto)
        {
            try
            {
                var data = this._business.SaveData(
                    createDto);

                return new ApiResponse<PessoaFisicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.Created,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaFisicaResponseDto>
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
        public ApiResponse<PessoaFisicaResponseDto> UpdatePessoaFisica([FromBody] PessoaFisicaRequestUpdateDto updateDto)
        {
            try
            {
                updateDto.Guid = (Guid)updateDto.Guid;

                var data = this._business.SaveData(
                    updateDto: updateDto);

                return new ApiResponse<PessoaFisicaResponseDto>
                {
                    Data = data,
                    Success = true,
                    StatusCode = HttpStatusCode.NoContent,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<PessoaFisicaResponseDto>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }
    }
}