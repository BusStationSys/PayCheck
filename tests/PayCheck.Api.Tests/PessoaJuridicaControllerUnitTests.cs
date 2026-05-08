namespace PayCheck.Api.Tests
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using PayCheck.Api.Controllers;

    public class PessoaJuridicaControllerUnitTests
    {
        private readonly Mock<IPessoaJuridicaService> _pessoaJuridicaServiceMock;

        private readonly PessoaJuridicaController _pessoaJuridicaController;

        public PessoaJuridicaControllerUnitTests()
        {
            this._pessoaJuridicaServiceMock = new Mock<IPessoaJuridicaService>();

            this._pessoaJuridicaController = new PessoaJuridicaController(
                this._pessoaJuridicaServiceMock.Object);
        }

        [Fact]
        public void GetPessoaJuridica_ShouldReturnOK_WhenFound()
        {
            //  Arrange
            var guid = Guid.NewGuid();

            var dto = new PessoaJuridicaResponseDto
            {
                Guid = guid
            };

            this._pessoaJuridicaServiceMock
                .Setup(s => s.Get(guid))
                .Returns(dto);

            // Act
            var result = this._pessoaJuridicaController.GetPessoaJuridica(
                guid);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsType<PessoaJuridicaResponseDto>(
                okObjectResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void GetPessoaJuridica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaJuridicaResponseDto?)null);

            // Act
            var notFoundObjectResult = this._pessoaJuridicaController.GetPessoaJuridica(
                guid);

            // Assert
            Assert.IsType<NotFoundObjectResult>(
                notFoundObjectResult);
        }

        [Fact]
        public void GetPessoaJuridica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaJuridicaController.GetPessoaJuridica(
                guid);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void GetPessoasJuridicas_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var dtos = new List<PessoaJuridicaResponseDto>
            {
                new() { Guid = Guid.NewGuid() },
                new() { Guid = Guid.NewGuid() }
            };

            this._pessoaJuridicaServiceMock.Setup(
                s => s.GetAll()).Returns(
                dtos);

            // Act
            var result = this._pessoaJuridicaController.GetPessoasJuridicas();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsAssignableFrom<IEnumerable<PessoaJuridicaResponseDto>>(
                okObjectResult.Value);

            Assert.Equal(
                2,
                returnValue.Count());
        }

        [Fact]
        public void GetPessoasJuridicas_ShouldReturnNotFound_WhenEmpty()
        {
            // Arrange
            this._pessoaJuridicaServiceMock.Setup(
                s => s.GetAll()).Returns(
                []);

            // Act
            var result = this._pessoaJuridicaController.GetPessoasJuridicas();

            // Assert
            Assert.IsType<NotFoundObjectResult>(
                result);
        }

        [Fact]
        public void GetPessoasJuridicas_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            this._pessoaJuridicaServiceMock.Setup(
                s => s.GetAll()).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaJuridicaController.GetPessoasJuridicas();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void DeletePessoaJuridica_ShouldReturnNoContent_WhenFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var dto = new PessoaJuridicaResponseDto
            {
                Guid = guid
            };

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                dto);

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Delete(
                    guid));

            // Act
            var result = this._pessoaJuridicaController.DeletePessoaJuridica(
                guid);

            // Assert
            Assert.IsType<NoContentResult>(
                result);
        }

        [Fact]
        public void DeletePessoaJuridica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaJuridicaResponseDto?)null);

            // Act
            var result = this._pessoaJuridicaController.DeletePessoaJuridica(
                guid);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(
                result);

            Assert.NotNull(
                notFoundObjectResult.Value);
        }

        [Fact]
        public void DeletePessoaJuridica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.NewGuid();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaJuridicaController.DeletePessoaJuridica(
                guid);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void CreatePessoaJuridica_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var createDto = new PessoaJuridicaRequestCreateDto();

            var responseDto = new PessoaJuridicaResponseDto
            {
                Guid = guid
            };

            this._pessoaJuridicaServiceMock.Setup(
                s => s.SaveData(
                    createDto,
                    null)).Returns(
                responseDto);

            // Act
            var result = this._pessoaJuridicaController.CreatePessoaJuridica(
                createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(
                result);

            var returnValue = Assert.IsType<PessoaJuridicaResponseDto>(
                createdResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void CreatePessoaJuridica_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = this._pessoaJuridicaController.CreatePessoaJuridica(
                null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(
                result);

            Assert.NotNull(
                badRequestResult.Value);
        }

        [Fact]
        public void CreatePessoaJuridica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var createDto = new PessoaJuridicaRequestCreateDto();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.SaveData(
                    createDto,
                    null)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaJuridicaController.CreatePessoaJuridica(
                createDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }

        [Fact]
        public void UpdatePessoaJuridica_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaJuridicaRequestUpdateDto();

            var responseDto = new PessoaJuridicaResponseDto
            {
                Guid = guid
            };

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                responseDto);

            this._pessoaJuridicaServiceMock.Setup(
                s => s.SaveData(
                    null,
                    updateDto)).Returns(
                responseDto);

            // Act
            var result = this._pessoaJuridicaController.UpdatePessoaJuridica(
                guid,
                updateDto);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(
                result);

            var returnValue = Assert.IsType<PessoaJuridicaResponseDto>(
                okObjectResult.Value);

            Assert.Equal(
                guid,
                returnValue.Guid);
        }

        [Fact]
        public void UpdatePessoaJuridica_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var result = this._pessoaJuridicaController.UpdatePessoaJuridica(
                guid,
                (PessoaJuridicaRequestUpdateDto?)null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(
                result);

            Assert.NotNull(
                badRequestResult.Value);
        }

        [Fact]
        public void UpdatePessoaJuridica_ShouldReturnNotFound_WhenNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaJuridicaRequestUpdateDto();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Returns(
                (PessoaJuridicaResponseDto?)null);

            // Act
            var result = this._pessoaJuridicaController.UpdatePessoaJuridica(
                guid,
                updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(
                result);

            Assert.NotNull(
                notFoundResult.Value);
        }

        [Fact]
        public void UpdatePessoaJuridica_ShouldReturnInternalServerError_WhenExceptionThrown()
        {
            // Arrange
            var guid = Guid.NewGuid();

            var updateDto = new PessoaJuridicaRequestUpdateDto();

            this._pessoaJuridicaServiceMock.Setup(
                s => s.Get(
                    guid)).Throws(
                new Exception(
                    "Erro inesperado."));

            // Act
            var result = this._pessoaJuridicaController.UpdatePessoaJuridica(
                guid,
                updateDto);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(
                result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                objectResult.StatusCode);
        }
    }
}