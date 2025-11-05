namespace PayCheck.Api.Controllers
{
    using ARVTech.DataAccess.DTOs;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using ARVTech.Shared.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Net;

    /// <summary>
    /// Provides API endpoints for managing payment statements.
    /// </summary>
    /// <remarks>This controller offers various endpoints to retrieve, update, and manage payment statements. It
    /// supports operations such as fetching payment statements by different criteria, updating payment statement details,
    /// and retrieving salary composition and evolution charts. All endpoints require authorization.</remarks>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class DemonstrativoPagamentoController : ControllerBase
    {
        private readonly IMatriculaDemonstrativoPagamentoService _service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemonstrativoPagamentoController"/> class.
        /// </summary>
        /// <param name="service">The service used to manage payment statements. Cannot be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="service"/> is <see langword="null"/>.</exception>
        public DemonstrativoPagamentoController(IMatriculaDemonstrativoPagamentoService service)
        {
            this._service = service ?? throw new ArgumentNullException(
                nameof(
                    service));
        }

        /// <summary>
        /// Retrieves the payment statement for a given identifier.
        /// </summary>
        /// <remarks>This method attempts to retrieve the payment statement associated with the specified <paramref
        /// name="guid"/>. If the payment statement is found, the response will include the data and a success status. If not
        /// found, the response will indicate a not found status. In case of an error, the response will contain the error
        /// message and an internal server error status.</remarks>
        /// <param name="guid">The unique identifier of the payment statement to retrieve.</param>
        /// <returns>An <see cref="ApiResponseDto{MatriculaDemonstrativoPagamentoResponseDto}"/> containing the payment statement data if
        /// found; otherwise, a response indicating that the payment statement was not found or an error occurred.</returns>
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
        /// Retrieves a collection of payment statements.
        /// </summary>
        /// <remarks>This method fetches all available payment statements and returns them in a successful response. If no
        /// payment statements are found, a not found response is returned. In case of an error, an internal server error
        /// response is returned.</remarks>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see
        /// cref="MatriculaDemonstrativoPagamentoResponseDto"/>. The response indicates success with a status code of <see
        /// cref="HttpStatusCode.OK"/> if data is found. If no data is found, the response indicates a status code of <see
        /// cref="HttpStatusCode.NotFound"/>. In case of an error, the response indicates a status code of <see
        /// cref="HttpStatusCode.InternalServerError"/>.</returns>
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
        /// Retrieves the payment statements associated with a specific collaborator identified by their GUID.
        /// </summary>
        /// <param name="guidColaborador">The unique identifier of the collaborator whose payment statements are to be retrieved.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see
        /// cref="MatriculaDemonstrativoPagamentoResponseDto"/> objects. If no payment statements are found, the response will
        /// include a message indicating this and a status code of <see cref="HttpStatusCode.NotFound"/>.</returns>
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
        /// Retrieves a collection of payment statements for a given competence and registration number.
        /// </summary>
        /// <remarks>This method returns an HTTP 200 status code with the data if successful, an HTTP 404 status code if
        /// no data is found, or an HTTP 500 status code if an error occurs.</remarks>
        /// <param name="competencia">The competence period for which the payment statements are requested. This is typically a string representing a date
        /// or period format.</param>
        /// <param name="matricula">The registration number associated with the payment statements. This is used to identify the specific account or
        /// individual.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see
        /// cref="MatriculaDemonstrativoPagamentoResponseDto"/> objects if found; otherwise, a response indicating that no
        /// payment statements were found.</returns>
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
        /// Retrieves a list of payment statement pending items within a specified date range and status.
        /// </summary>
        /// <remarks>This method queries pending payment statements based on the provided date range and status. If no
        /// pending items are found, the response will indicate this with a <see cref="HttpStatusCode.NotFound"/>
        /// status.</remarks>
        /// <param name="periodoInicial">The start date of the period to search for pending items. Must be a valid <see cref="DateTime"/>.</param>
        /// <param name="periodoFinal">The end date of the period to search for pending items. Must be a valid <see cref="DateTime"/>.</param>
        /// <param name="situacao">The status of the pending items to filter by. Defaults to <see
        /// cref="SituacaoPendenciaDemonstrativoPagamento.Todos"/>.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see
        /// cref="MatriculaDemonstrativoPagamentoResponseDto"/> if pending items are found; otherwise, a message indicating no
        /// items were found.</returns>
        [HttpGet("Pendencias")]
        public ApiResponseDto<IEnumerable<MatriculaDemonstrativoPagamentoResponseDto>> GetPendencias(
            [FromQuery] DateTime periodoInicial,
            [FromQuery] DateTime periodoFinal,
            [FromQuery] SituacaoPendenciaDemonstrativoPagamento situacao = SituacaoPendenciaDemonstrativoPagamento.Todos)
        {
            try
            {
                var data = this._service.GetPendencias(
                    Convert.ToDateTime(
                        periodoInicial),
                    Convert.ToDateTime(
                        periodoFinal),
                    situacao);

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
        /// Retrieves the salary composition chart for a specified user and period.
        /// </summary>
        /// <param name="guidUsuario">The unique identifier of the user for whom the salary composition chart is requested.</param>
        /// <param name="competencia">The period for which the salary composition chart is requested, typically in a format like "YYYYMM".</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see
        /// cref="GraficoComposicaoSalarialResponseDto"/> objects representing the salary composition chart. If no data
        /// is found, the response will indicate a not found status with an appropriate message.</returns>
        [HttpGet("GraficoComposicaoSalarial/{guidUsuario}/{competencia}")]
        public ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>> GetGraficoComposicaoSalarial(Guid guidUsuario, string competencia)
        {
            try
            {
                var data = this._service.GetSalaryCompositionChart(
                    guidUsuario,
                    competencia);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>>
                {
                    Message = $"Gráfico de Composição Salarial não encontrado para o Usuário {guidUsuario}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<GraficoComposicaoSalarialResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Retrieves the salary evolution chart data for a specified user over a given number of past months.
        /// </summary>
        /// <param name="guidUsuario">The unique identifier of the user for whom the salary evolution chart is requested.</param>
        /// <param name="quantidadeMesesRetroativos">The number of months to look back for salary evolution data.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing a collection of <see cref="GraficoEvolucaoSalarialResponseDto"/>
        /// objects representing the salary evolution chart data. If no data is found, the response indicates a not found
        /// status.</returns>
        [HttpGet("GraficoEvolucaoSalarial/{guidUsuario}/{quantidadeMesesRetroativos}")]
        public ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>> GetGraficoEvolucaoSalarial(Guid guidUsuario, Int16 quantidadeMesesRetroativos)
        {
            try
            {
                var data = this._service.GetSalaryEvolutionChart(
                    guidUsuario,
                    quantidadeMesesRetroativos);

                if (data != null &&
                    data.Count() > 0)
                    return new ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>>
                    {
                        Data = data,
                        Success = true,
                        StatusCode = HttpStatusCode.OK,
                    };

                return new ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>>
                {
                    Message = $"Gráfico de Evolução Salarial não encontrado para o Usuário {guidUsuario}!",
                    StatusCode = HttpStatusCode.NotFound,
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<IEnumerable<GraficoEvolucaoSalarialResponseDto>>
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                };
            }
        }

        /// <summary>
        /// Updates the payment statement for a specific enrollment identified by the provided GUID.
        /// </summary>
        /// <remarks>This method updates the payment statement for the specified enrollment and returns a response
        /// indicating the success or failure of the operation. The response includes the updated payment statement details if
        /// the operation is successful.</remarks>
        /// <param name="guid">The unique identifier of the enrollment whose payment statement is to be updated.</param>
        /// <param name="updateDto">The data transfer object containing the updated payment statement details.</param>
        /// <returns>An <see cref="ApiResponseDto{T}"/> containing the updated payment statement details and the operation status.</returns>
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