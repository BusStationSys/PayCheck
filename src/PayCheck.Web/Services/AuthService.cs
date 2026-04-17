namespace PayCheck.Web.Services
{
    using ARVTech.DataAccess.DTOs.UniPayCheck;
    using Newtonsoft.Json;
    using PayCheck.Web.Infrastructure.Http.Interfaces;
    using PayCheck.Web.Services.Interfaces;

    public class AuthService : IAuthService
    {
        private string _token;

        private DateTime _dataExpiracao;

        private readonly IHttpClientService _httpClientService;

        public AuthService(IHttpClientService httpClientService)
        {
            this._httpClientService = httpClientService;
        }

        /// <summary>
        /// Asynchronously retrieves a valid authentication token for use in calls to the protected API.
        /// </summary>
        /// <remarks>The returned token is cached until its expiration to optimize subsequent calls.
        /// It is recommended to handle possible authentication failures when consuming this method.</remarks>
        /// <returns>A string containing the authentication token. The value will be reused if it is still valid;
        /// otherwise, a new token will be obtained.</returns>
        /// <exception cref="Exception">Thrown if an error occurs during the authentication process or if the API response indicates failure.</exception>
        public async Task<string> GetTokenAsync()
        {
            //  Se ainda é válido, reutiliza.
            if (!string.IsNullOrEmpty(
                this._token) &&
                this._dataExpiracao > DateTime.UtcNow)
                return this._token;

            var username = "arvtech";   //   _externalApis.Auth.Username;
            var password = "(@rV73Ch)"; //   _externalApis.Auth.Password;

            var authDto = new AuthRequestDto
            {
                Username = username,
                Password = password
            };

            var json = JsonConvert.SerializeObject(
                authDto,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            this._httpClientService.SetBasicAuthentication(
                username,
                password);

            using (var httpResponseMessage = await this._httpClientService.ExecuteAsync(
                HttpMethod.Post,
                "auth",
                json))
            {
                if (!httpResponseMessage.IsSuccessStatusCode)
                    throw new Exception("Erro ao autenticar.");

                var responseJson = await httpResponseMessage.Content.ReadAsStringAsync();

                var authResponse = JsonConvert.DeserializeObject<AuthResponseDto>(
                    responseJson);

                this._token = authResponse.Token;

                this._dataExpiracao = DateTime.UtcNow.AddMinutes(30);

                return _token;
            }
        }
    }
}