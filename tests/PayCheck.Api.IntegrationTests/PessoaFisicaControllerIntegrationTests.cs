namespace PayCheck.Api.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Moq;

    public class PessoaFisicaControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<IPessoaFisicaService> _serviceMock;

        private readonly string _baseUrl = @"/api/PessoaFisica";

        public PessoaFisicaControllerIntegrationTests(CustomWebApplicationFactory customWebApplicationFactory)
        {
            this._httpClient = customWebApplicationFactory.CreateClient();
            this._serviceMock = customWebApplicationFactory.PessoaFisicaServiceMock;
        }

        [Fact]
        public async Task GetAniversariantes_WhenRequestIsValid_ShouldReturn200Ok()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAniversariantes(
                    "0101",
                    "1231")).Returns(
                new List<PessoaFisicaResponseDto>
                {
                    new()
                    {
                        Guid = Guid.NewGuid(),
                        Nome = "Fulano de Tal",
                    },
                });

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=0101&periodoFinalString=1231"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.OK,
                    httpResponseMessage.StatusCode);

                var content = await httpResponseMessage.Content.ReadFromJsonAsync<List<PessoaFisicaResponseDto>>();

                Assert.NotNull(content);
                Assert.NotEmpty(content);
            }
        }
    }
}