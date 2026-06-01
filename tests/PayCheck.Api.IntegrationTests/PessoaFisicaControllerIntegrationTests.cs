namespace PayCheck.Api.Integration.Test
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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
        public async Task DeletePessoaFisica_WhenFound_ShouldReturn204NoContent()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                new PessoaFisicaResponseDto { Guid = guid });

            this._serviceMock.Setup(
                s => s.Delete(guid));

            // Act
            using (var httpResponseMessage = await this._httpClient.DeleteAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NoContent,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task DeletePessoaFisica_WhenNotFound_ShouldReturn404NotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            using (var httpResponseMessage = await this._httpClient.DeleteAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task DeletePessoaFisica_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.DeleteAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
        }

        [Fact]
        public async Task GetPessoaFisica_WhenFound_ShouldReturn200Ok()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                new PessoaFisicaResponseDto
                {
                    Guid = guid,
                    Nome = "Fulano de Tal",
                });

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.OK,
                    httpResponseMessage.StatusCode);

                var content = await httpResponseMessage.Content.ReadFromJsonAsync<PessoaFisicaResponseDto>();

                Assert.NotNull(content);
                Assert.Equal(guid, content.Guid);
            }
        }

        [Fact]
        public async Task GetPessoaFisica_WhenNotFound_ShouldReturn404NotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetPessoaFisica_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._serviceMock.Setup(
                s => s.Get(guid)).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                $"{this._baseUrl}/{guid}"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
        }

        [Fact]
        public async Task GetPessoasFisicas_WhenFound_ShouldReturn200Ok()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAll()).Returns(
                new List<PessoaFisicaResponseDto>
                {
                    new() { Guid = Guid.NewGuid(), Nome = "Fulano" },
                    new() { Guid = Guid.NewGuid(), Nome = "Ciclano" },
                });

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                this._baseUrl))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.OK,
                    httpResponseMessage.StatusCode);

                var content = await httpResponseMessage.Content.ReadFromJsonAsync<List<PessoaFisicaResponseDto>>();

                Assert.NotNull(content);
                Assert.Equal(2, content.Count);
            }
        }

        [Fact]
        public async Task GetPessoasFisicas_WhenEmpty_ShouldReturn404NotFound()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAll()).Returns(
                Array.Empty<PessoaFisicaResponseDto>());

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                this._baseUrl))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetPessoasFisicas_WhenNull_ShouldReturn404NotFound()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAll()).Returns(
                (IEnumerable<PessoaFisicaResponseDto>?)null);

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                this._baseUrl))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetPessoasFisicas_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAll()).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                this._baseUrl))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
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

        [Fact]
        public async Task GetAniversariantes_WhenEmpty_ShouldReturn404NotFound()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAniversariantes(
                    "0601",
                    "3006")).Returns(
                Array.Empty<PessoaFisicaResponseDto>());

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=0601&periodoFinalString=3006"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetAniversariantes_WhenNull_ShouldReturn404NotFound()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAniversariantes(
                    "0701",
                    "3107")).Returns(
                (IEnumerable<PessoaFisicaResponseDto>?)null);

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=0701&periodoFinalString=3107"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetAniversariantes_WhenPeriodoInicialIsEmpty_ShouldReturn400BadRequest()
        {
            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=&periodoFinalString=1231"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetAniversariantes_WhenPeriodoFinalIsEmpty_ShouldReturn400BadRequest()
        {
            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=0101&periodoFinalString="))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetAniversariantes_WhenParametersAreWhiteSpace_ShouldReturn400BadRequest()
        {
            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=   &periodoFinalString=   "))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task GetAniversariantes_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            this._serviceMock.Setup(
                s => s.GetAniversariantes(
                    "0101",
                    "3112")).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.GetAsync(
                @$"{this._baseUrl}/Aniversariantes?periodoInicialString=0101&periodoFinalString=3112"))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
        }

        [Fact]
        public async Task CreatePessoaFisica_WhenValid_ShouldReturn201Created()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var createDto = new PessoaFisicaRequestCreateDto
            {
                Nome = "Fulano de Tal",
                DataNascimento = new DateTime(1990, 1, 1),
            };

            this._serviceMock.Setup(
                s => s.SaveData(
                    It.IsAny<PessoaFisicaRequestCreateDto>(),
                    null)).Returns(
                new PessoaFisicaResponseDto
                {
                    Guid = guid,
                    Nome = createDto.Nome,
                });

            // Act
            using (var httpResponseMessage = await this._httpClient.PostAsJsonAsync(
                this._baseUrl,
                createDto))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.Created,
                    httpResponseMessage.StatusCode);

                var content = await httpResponseMessage.Content.ReadFromJsonAsync<PessoaFisicaResponseDto>();

                Assert.NotNull(content);
                Assert.Equal(guid, content.Guid);
            }
        }

        [Fact]
        public async Task CreatePessoaFisica_WhenDtoIsNull_ShouldReturn400BadRequest()
        {
            // Act
            using (var httpResponseMessage = await this._httpClient.PostAsJsonAsync<PessoaFisicaRequestCreateDto?>(
                this._baseUrl,
                null))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task CreatePessoaFisica_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            var createDto = new PessoaFisicaRequestCreateDto
            {
                Nome = "Fulano de Tal",
                DataNascimento = new DateTime(1990, 1, 1),
            };

            this._serviceMock.Setup(
                s => s.SaveData(
                    It.IsAny<PessoaFisicaRequestCreateDto>(),
                    null)).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.PostAsJsonAsync(
                this._baseUrl,
                createDto))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
        }

        [Fact]
        public async Task UpdatePessoaFisica_WhenFound_ShouldReturn200Ok()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto
            {
                Nome = "Fulano Atualizado",
                DataNascimento = DateTime.Today,
            };

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                new PessoaFisicaResponseDto { Guid = guid });

            this._serviceMock.Setup(
                s => s.SaveData(
                    null,
                    It.Is<PessoaFisicaRequestUpdateDto>(d => d.Guid == guid))).Returns(
                new PessoaFisicaResponseDto
                {
                    Guid = guid,
                    Nome = updateDto.Nome,
                });

            // Act
            using (var httpResponseMessage = await this._httpClient.PutAsJsonAsync(
                $"{this._baseUrl}/{guid}",
                updateDto))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.OK,
                    httpResponseMessage.StatusCode);

                var content = await httpResponseMessage.Content.ReadFromJsonAsync<PessoaFisicaResponseDto>();

                Assert.NotNull(content);
                Assert.Equal(guid, content.Guid);
            }
        }

        [Fact]
        public async Task UpdatePessoaFisica_WhenDtoIsNull_ShouldReturn400BadRequest()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            using (var httpResponseMessage = await this._httpClient.PutAsJsonAsync<PessoaFisicaRequestUpdateDto?>(
                $"{this._baseUrl}/{guid}",
                null))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.BadRequest,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task UpdatePessoaFisica_WhenNotFound_ShouldReturn404NotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto
            {
                Nome = "Fulano Atualizado",
                DataNascimento = new DateTime(1990, 1, 1),
            };

            this._serviceMock.Setup(
                s => s.Get(guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            using (var httpResponseMessage = await this._httpClient.PutAsJsonAsync(
                $"{this._baseUrl}/{guid}",
                updateDto))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.NotFound,
                    httpResponseMessage.StatusCode);
            }
        }

        [Fact]
        public async Task UpdatePessoaFisica_WhenExceptionThrown_ShouldReturn500InternalServerError()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto
            {
                Nome = "Fulano Atualizado",
                DataNascimento = DateTime.Today,
            };

            this._serviceMock.Setup(
                s => s.Get(guid)).Throws(
                new Exception("Erro inesperado."));

            // Act
            using (var httpResponseMessage = await this._httpClient.PutAsJsonAsync(
                $"{this._baseUrl}/{guid}",
                updateDto))
            {
                // Assert
                Assert.Equal(
                    HttpStatusCode.InternalServerError,
                    httpResponseMessage.StatusCode);

                var problem = await httpResponseMessage.Content.ReadFromJsonAsync<ProblemDetails>();

                Assert.NotNull(problem);
                Assert.Equal("Erro inesperado.", problem.Detail);
                Assert.Equal(StatusCodes.Status500InternalServerError, problem.Status);
            }
        }
    }
}