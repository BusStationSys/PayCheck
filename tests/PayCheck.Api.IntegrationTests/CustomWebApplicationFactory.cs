namespace PayCheck.Api.IntegrationTests
{
    using ARVTech.DataAccess.Service.UniPayCheck.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Moq;

    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IPessoaFisicaService> PessoaFisicaServiceMock { get; } = new Mock<IPessoaFisicaService>();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                // Substitui o serviço real (que usa banco de dados) pelo Mock.
                services.RemoveAll<IPessoaFisicaService>();

                services.AddScoped(_ => PessoaFisicaServiceMock.Object);

                // Substitui o JWT por um esquema que sempre autentica.
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            });
        }
    }
}