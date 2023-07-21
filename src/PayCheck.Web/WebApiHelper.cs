namespace PayCheck.Web
{
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Net;
    using System.Text;
    using System;
    using ARVTech.Shared;

    public class WebApiHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly string _requestUri;
        private readonly string _username;
        private readonly string _password;

        private bool _disposedValue = false;

        public WebApiHelper(string requestUri, string username, string password)
        {
            _httpClient = new HttpClient();

            this._requestUri = requestUri;

            this._username = username;
            this._password = password;
        }

        public string ExecutePost(bool authorizeAttribute = false)
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = default(HttpResponseMessage);

                if (authorizeAttribute)
                {
                    //  Limpa o Header.
                    this._httpClient.DefaultRequestHeaders.Accept.Clear();

                    httpResponseMessage = this._httpClient.PostAsync(
                        this._requestUri,
                        new StringContent(
                            JsonConvert.SerializeObject(new
                            {
                                Username = this._username,
                                Password = this._password
                            }),
                        Encoding.UTF8,
                        Common.MediaTypes)).Result;
                }
                else
                {
                    httpResponseMessage = this._httpClient.PostAsync(
                        this._requestUri,
                        null).Result;
                }

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    // Obtém o Token Gerado.
                    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                    ////deserializa o token e data de expiração para o objeto Token
                    //Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                    //// Associar o token aos headers do objeto
                    //// do tipo HttpClient
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                }
                else
                {
                    throw new Exception(httpResponseMessage.StatusCode.ToString());
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this._httpClient.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                this._disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WebApiHelper()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}