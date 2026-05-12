namespace PayCheck.Api.Tests
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using PayCheck.Api.Controllers;

    public class PessoaFisicaControllerUnitTests
    {
        private readonly Mock<IPessoaFisicaService> _pessoaFisicaServiceMock;

        private readonly PessoaFisicaController _pessoaFisicaController;

        public PessoaFisicaControllerUnitTests()
        {
            this._pessoaFisicaServiceMock = new Mock<IPessoaFisicaService>();

            this._pessoaFisicaController = new PessoaFisicaController(
                this._pessoaFisicaServiceMock.Object);
        }

        [Fact]
        public void GetPessoaFisica_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var dto = new PessoaFisicaResponseDto
            {
                Guid = guid
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                dto);

            // Act
            var result = this._pessoaFisicaController.GetPessoaFisica(
                guid);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsType<PessoaFisicaResponseDto>(
                okObjectResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void GetPessoaFisica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            var result = this._pessoaFisicaController.GetPessoaFisica(
                guid);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(
                result);

            Assert.NotNull(
                notFoundObjectResult.Value);
        }

        [Fact]
        public void GetPessoaFisica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaFisicaController.GetPessoaFisica(
                guid);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void DeletePessoaFisica_ShouldReturnNoContent_WhenFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var dto = new PessoaFisicaResponseDto
            {
                Guid = guid
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                dto);

            this._pessoaFisicaServiceMock.Setup(
                s => s.Delete(
                    guid));

            // Act
            var result = this._pessoaFisicaController.DeletePessoaFisica(
                guid);

            // Assert
            Assert.IsType<NoContentResult>(
                result);
        }

        [Fact]
        public void DeletePessoaFisica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            var result = this._pessoaFisicaController.DeletePessoaFisica(
                guid);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(
                result);

            Assert.NotNull(
                notFoundObjectResult.Value);
        }

        [Fact]
        public void GetPessoasFisicas_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var dtos = new List<PessoaFisicaResponseDto>
            {
                new() { Guid = Guid.NewGuid() },
                new() { Guid = Guid.NewGuid() }
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.GetAll()).Returns(
                dtos);

            // Act
            var result = this._pessoaFisicaController.GetPessoasFisicas();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaFisicaResponseDto>>(
                okObjectResult.Value);

            Assert.Equal(
                2,
                returnValue.Count());
        }

        [Fact]
        public void GetPessoasFisicas_ShouldReturnNotFound_WhenEmpty()
        {
            // Arrange
            this._pessoaFisicaServiceMock.Setup(
                s => s.GetAll()).Returns(
                    Array.Empty<PessoaFisicaResponseDto>());

            // Act
            var result = this._pessoaFisicaController.GetPessoasFisicas();

            // Assert
            Assert.IsType<NotFoundObjectResult>(
                result);
        }

        [Fact]
        public void CreatePessoaFisica_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var createDto = new PessoaFisicaRequestCreateDto();

            var responseDto = new PessoaFisicaResponseDto
            {
                Guid = guid
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.SaveData(
                    createDto,
                    null)).Returns(
                responseDto);

            // Act
            var result = this._pessoaFisicaController.CreatePessoaFisica(
                createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(
                result);

            var returnValue = Assert.IsType<PessoaFisicaResponseDto>(
                createdResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void CreatePessoaFisica_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = this._pessoaFisicaController.CreatePessoaFisica(
                null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(
                result);

            Assert.NotNull(
                badRequestResult.Value);
        }

        [Fact]
        public void CreatePessoaFisica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new PessoaFisicaRequestCreateDto();

            this._pessoaFisicaServiceMock.Setup(
                s => s.SaveData(
                    createDto,
                    null)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaFisicaController.CreatePessoaFisica(
                createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void UpdatePessoaFisica_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto();

            var responseDto = new PessoaFisicaResponseDto
            {
                Guid = guid,
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                responseDto);

            this._pessoaFisicaServiceMock.Setup(
                s => s.SaveData(
                    null,
                    updateDto)).Returns(
                responseDto);

            // Act
            var result = this._pessoaFisicaController.UpdatePessoaFisica(
                guid,
                updateDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsType<PessoaFisicaResponseDto>(
                okObjectResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void UpdatePessoaFisica_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = this._pessoaFisicaController.UpdatePessoaFisica(
                guid,
                null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(
                result);

            Assert.NotNull(
                badRequestResult.Value);
        }

        [Fact]
        public void UpdatePessoaFisica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto();

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaFisicaResponseDto?)null);

            // Act
            var result = this._pessoaFisicaController.UpdatePessoaFisica(
                guid,
                updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(
                result);

            Assert.NotNull(
                notFoundResult.Value);
        }

        [Fact]
        public void UpdatePessoaFisica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaFisicaRequestUpdateDto();

            this._pessoaFisicaServiceMock.Setup(
                s => s.Get(
                    guid)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaFisicaController.UpdatePessoaFisica(
                guid,
                updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void GetAniversariantes_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var dtos = new List<PessoaFisicaResponseDto>
            {
                new()
                {
                    Guid = Guid.NewGuid(),
                }
            };

            this._pessoaFisicaServiceMock.Setup(
                s => s.GetAniversariantes(
                    "0101",
                    "3112")).Returns(
                dtos);

            // Act
            var result = this._pessoaFisicaController.GetAniversariantes(
                "0101",
                "3112");

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaFisicaResponseDto>>(
                okObjectResult.Value);

            Assert.Single(
                returnValue);
        }

        [Fact]
        public void GetAniversariantes_ShouldReturnNotFound_WhenEmpty()
        {
            // Arrange
            this._pessoaFisicaServiceMock.Setup(
                s => s.GetAniversariantes(
                    "0101",
                    "3112"))
                .Returns(
                    Array.Empty<PessoaFisicaResponseDto>());

            // Act
            var result = this._pessoaFisicaController.GetAniversariantes(
                "0101",
                "3112");

            // Assert
            Assert.IsType<NotFoundObjectResult>(
                result);
        }

        [Fact]
        public void GetAniversariantes_ShouldReturnBadRequest_WhenParametersAreEmpty()
        {
            // Act
            var result = this._pessoaFisicaController.GetAniversariantes(
                string.Empty,
                string.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(
                result);

            Assert.NotNull(
                badRequestResult.Value);
        }
    }
}