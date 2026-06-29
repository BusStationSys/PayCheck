namespace PayCheck.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.Contracts.PayCheck.Requests.Update;
    using ARVTech.DataAccess.Contracts.PayCheck.Responses;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using ARVTech.Shared.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

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
        /// Retrieves a payment statement by its identifier.
        /// </summary>
        /// <param name="guid">The unique identifier of the payment statement.</param>
        /// <returns>
        /// A <see cref="MatriculaDemonstrativoPagamentoResponse"/> when found.
        /// Returns <c>404 Not Found</c> when the resource does not exist.
        /// </returns>
        [HttpGet("{guid}")]
        [ProducesResponseType(typeof(MatriculaDemonstrativoPagamentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDemonstrativoPagamentoAsync(Guid guid)
        {
            var data = await Task.FromResult(
                this._service.Get(
                    guid));

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Demonstrativo de Pagamento {guid} não encontrado!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves all payment statements.
        /// </summary>
        /// <remarks>
        /// Returns a collection of payment statements when available.
        /// </remarks>
        /// <returns>
        /// A collection of <see cref="MatriculaDemonstrativoPagamentoResponse"/>.
        /// Returns <c>200 OK</c> when data is found, or <c>404 Not Found</c> when no records exist.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MatriculaDemonstrativoPagamentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDemonstrativosPagamentoAsync()
        {
            var data = await Task.FromResult(
                this._service.GetAll());

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = "Demonstrativos de Pagamento não encontrados!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves payment statements for a specific collaborator.
        /// </summary>
        /// <param name="guidColaborador">The collaborator identifier.</param>
        /// <returns>
        /// A collection of <see cref="MatriculaDemonstrativoPagamentoResponse"/>.
        /// Returns <c>404 Not Found</c> when no records exist.
        /// </returns>
        [HttpGet("Colaborador/{guidColaborador}")]
        [ProducesResponseType(typeof(IEnumerable<MatriculaDemonstrativoPagamentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDemonstrativoPagamentoByGuidColaboradorAsync(Guid guidColaborador)
        {
            var data = await Task.FromResult(
                this._service.GetByGuidColaborador(
                    guidColaborador));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Demonstrativos de Pagamento não encontrados para o Colaborador {guidColaborador}.",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves payment statements by competence and registration.
        /// </summary>
        /// <param name="competencia">The competence period (e.g., YYYYMM).</param>
        /// <param name="matricula">The registration identifier.</param>
        /// <returns>
        /// A collection of <see cref="MatriculaDemonstrativoPagamentoResponse"/>.
        /// Returns <c>404 Not Found</c> when no records exist.
        /// </returns>
        [HttpGet("{competencia}/{matricula}")]
        [ProducesResponseType(typeof(IEnumerable<MatriculaDemonstrativoPagamentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDemonstrativoPagamentoByCompetenciaAndMatricula(string competencia, string matricula)
        {
            var data = await Task.FromResult(
                this._service.Get(
                    competencia,
                    matricula));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Demonstrativos de Pagamento não encontrados para a Competência {competencia} e Matrícula {matricula}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves pending payment statements within a specified period and status.
        /// </summary>
        /// <param name="periodoInicial">The start date of the search period.</param>
        /// <param name="periodoFinal">The end date of the search period.</param>
        /// <param name="situacao">The status filter for pending items.</param>
        /// <returns>
        /// A collection of <see cref="MatriculaDemonstrativoPagamentoResponse"/>.
        /// Returns <c>404 Not Found</c> when no records exist.
        /// </returns>
        [HttpGet("Pendencias")]
        [ProducesResponseType(typeof(IEnumerable<MatriculaDemonstrativoPagamentoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPendenciasAsync(
            [FromQuery] DateTime periodoInicial,
            [FromQuery] DateTime periodoFinal,
            [FromQuery] SituacaoPendenciaDemonstrativoPagamento situacao = SituacaoPendenciaDemonstrativoPagamento.Todos)
        {
            var data = await Task.FromResult(
                this._service.GetPendencias(
                    periodoInicial,
                    periodoFinal,
                    situacao));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Pendências de Demonstrativos de Pagamento não encontrados para o Período de {periodoInicial} a {periodoFinal}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves the salary composition chart for a specific user and competence.
        /// </summary>
        /// <param name="guidUsuario">The user identifier.</param>
        /// <param name="competencia">The competence period (e.g., YYYYMM).</param>
        /// <returns>
        /// A collection of <see cref="GraficoComposicaoSalarialResponse"/> representing the salary composition.
        /// Returns <c>404 Not Found</c> when no data is available.
        /// </returns>
        [HttpGet("GraficoComposicaoSalarial/{guidUsuario}/{competencia}")]
        [ProducesResponseType(typeof(IEnumerable<GraficoComposicaoSalarialResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetGraficoComposicaoSalarialAsync(Guid guidUsuario, string competencia)
        {
            var data = await Task.FromResult(
                this._service.GetSalaryCompositionChart(
                    guidUsuario,
                    competencia));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Gráfico de Composição Salarial não encontrado para o Usuário {guidUsuario}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Retrieves the salary evolution chart for a specific user over a given number of months.
        /// </summary>
        /// <param name="guidUsuario">The user identifier.</param>
        /// <param name="quantidadeMesesRetroativos">The number of past months to include in the evolution.</param>
        /// <returns>
        /// A collection of <see cref="GraficoEvolucaoSalarialResponse"/> representing the salary evolution.
        /// Returns <c>404 Not Found</c> when no data is available.
        /// </returns>
        [HttpGet("GraficoEvolucaoSalarial/{guidUsuario}/{quantidadeMesesRetroativos}")]
        [ProducesResponseType(typeof(IEnumerable<GraficoEvolucaoSalarialResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetGraficoEvolucaoSalarialAsync(Guid guidUsuario, Int16 quantidadeMesesRetroativos)
        {
            var data = await Task.FromResult(
                this._service.GetSalaryEvolutionChart(
                    guidUsuario,
                    quantidadeMesesRetroativos));

            if (data is null ||
                !data.Any())
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Gráfico de Evolução Salarial não encontrado para o Usuário {guidUsuario}!",
                    });

            return Ok(
                data);
        }

        /// <summary>
        /// Updates a payment statement identified by its GUID.
        /// </summary>
        /// <param name="guid">The payment statement identifier.</param>
        /// <param name="updateRequest">The data used to update the payment statement.</param>
        /// <returns>
        /// The updated <see cref="MatriculaDemonstrativoPagamentoResponse"/>.
        /// </returns>
        [HttpPut("{guid}")]
        [ProducesResponseType(typeof(MatriculaDemonstrativoPagamentoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDemonstrativoPagamentoAsync(Guid guid, [FromBody] MatriculaDemonstrativoPagamentoUpdateRequest updateRequest)
        {
            if (updateRequest is null)
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "Bad Request",
                        Detail = "Payload inválido."
                    });

            updateRequest.Guid = guid;

            var data = await Task.FromResult(
                this._service.SaveData(
                    updateRequest: updateRequest));

            if (data is null)
                return NotFound(
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Demonstrativo de Pagamento {guid} não encontrado.",
                    });

            return Ok(
                data);
        }
    }
}