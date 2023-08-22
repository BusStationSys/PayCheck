namespace PayCheck.Web
{
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Net;
    using System.Text;
    using System;
    using ARVTech.Shared;
    using System.Net.Http;
    using System.Web.Http;

    public class WebApiHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly string _requestUri;
        private readonly string _username;
        private readonly string _password;
        private readonly string _token;

        private bool _disposedValue = false;

        public WebApiHelper(string requestUri, string username, string password)
        {
            this._httpClient = new HttpClient();

            this._requestUri = requestUri;

            this._username = username;
            this._password = password;
        }

        public WebApiHelper(string requestUri, string token)
        {
            this._httpClient = new HttpClient();

            this._requestUri = requestUri;

            this._token = token;
        }

        public string ExecuteGetWithoutAuthentication()
        {
            try
            {
                var httpResponseMessage = this._httpClient.GetAsync(
                    this._requestUri).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }

                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    // Obtém o Token Gerado.
                //    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                //    ////deserializa o token e data de expiração para o objeto Token
                //    //Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                //    //// Associar o token aos headers do objeto
                //    //// do tipo HttpClient
                //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                //}
                //else
                //{
                //    throw new Exception(httpResponseMessage.StatusCode.ToString());
                //}

                return string.Empty;
            }
            catch
            {
                throw;
            }
        }

        public string ExecuteGetAuthenticationByBasic()
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                this._httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(
                    this._username,
                    this._password);

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.GetAsync(
                    this._requestUri).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }

                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    // Obtém o Token Gerado.
                //    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                //    ////deserializa o token e data de expiração para o objeto Token
                //    //Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                //    //// Associar o token aos headers do objeto
                //    //// do tipo HttpClient
                //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                //}
                //else
                //{
                //    throw new Exception(httpResponseMessage.StatusCode.ToString());
                //}

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExecuteGetAuthenticationByBearer()
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    this._token);

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.GetAsync(
                    this._requestUri).Result;

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }

                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    // Obtém o Token Gerado.
                //    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                //    ////deserializa o token e data de expiração para o objeto Token
                //    //Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                //    //// Associar o token aos headers do objeto
                //    //// do tipo HttpClient
                //    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                //}
                //else
                //{
                //    throw new Exception(httpResponseMessage.StatusCode.ToString());
                //}

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExecutePostAuthenticationByBasic(object content)
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                this._httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(
                    this._username,
                    this._password);

                JsonContent jsonContent = JsonContent.Create(
                    content);

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.PostAsync(
                    this._requestUri,
                    jsonContent).Result;

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    // Obtém o Token Gerado.
                    return httpResponseMessage.Content.ReadAsStringAsync().Result;

                    ////deserializa o token e data de expiração para o objeto Token
                    //Token token = JsonConvert.DeserializeObject<Token>(conteudo);

                    //// Associar o token aos headers do objeto
                    //// do tipo HttpClient
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", token.AccessToken);
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

/*
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        this._mediaTypes));

                //new MediaTypeWithQualityHeaderValue(
                //    "text/plain"));

                this._httpClient.DefaultRequestHeaders.Add(
                    "Authorization",
                    $@"Basic {Common.GetTokenBase64Encode(
                        this._username,
                        this._password)}");
*/

        public string ExecutePostAuthenticationByBearer(object content)
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    this._token);

                JsonContent jsonContent = JsonContent.Create(
                    content);

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.PostAsync(
                    this._requestUri,
                    jsonContent).Result;

                return httpResponseMessage.Content.ReadAsStringAsync().Result;

                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    // Retorna o resultado do consumo do Post.
                //    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                //}
                //else
                //{
                //    string errorMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;

                //    throw new HttpResponseException(
                //        httpResponseMessage);

                //    //throw new Exception(httpResponseMessage.StatusCode.ToString());
                //}

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExecutePutAuthenticationByBearer(object content)
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                this._httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Bearer",
                    this._token);

                JsonContent jsonContent = JsonContent.Create(
                    content);

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.PutAsync(
                    this._requestUri,
                    jsonContent).Result;

                return httpResponseMessage.Content.ReadAsStringAsync().Result;

                //if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                //{
                //    // Retorna o resultado do consumo do Post.
                //    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                //}
                //else
                //{
                //    string errorMessage = httpResponseMessage.Content.ReadAsStringAsync().Result;

                //    throw new HttpResponseException(
                //        httpResponseMessage);

                //    //throw new Exception(httpResponseMessage.StatusCode.ToString());
                //}

                return string.Empty;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string ExecutePostWithoutAuthentication()
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        Common.MediaTypes));

                //  Limpa o Header.
                this._httpClient.DefaultRequestHeaders.Accept.Clear();

                // Envio da requisição a fim de autenticar e obter o token de acesso.
                var httpResponseMessage = this._httpClient.PostAsync(
                    this._requestUri,
                    null).Result;

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