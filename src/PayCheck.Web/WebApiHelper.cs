namespace PayCheck.Web
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using ARVTech.Shared;

    public class WebApiHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly string _requestUri;
        private readonly string _username;
        private readonly string _password;
        private readonly string _token;

        public readonly string _mediaTypes = @"application/json";

        private bool _disposedValue = false;

        public WebApiHelper(string requestUri)
        {
            this._httpClient = new HttpClient();

            this._requestUri = requestUri;
        }

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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ExecuteGetWithoutAuthentication()
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                                    HttpMethod.Get,
                                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    using (var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result)
                    {
                        try
                        {
                            httpResponseMessage.EnsureSuccessStatusCode();
                        }
                        catch
                        {
                            throw new Exception(
                                string.Concat(
                                    Convert.ToInt16(
                                        httpResponseMessage.StatusCode),
                                    " ",
                                    httpResponseMessage.ReasonPhrase));
                        }

                        return httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ExecuteGetWithAuthenticationByBasic()
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get,
                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    httpRequestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(
                            this._mediaTypes));

                    httpRequestMessage.Headers.Add(
                        "Authorization",
                        $"Basic {Common.GetTokenBase64Encode(this._username, this._password)}");

                    //        this._httpClient.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(
                    //            this._username,
                    //            this._password);

                    //httpRequestMessage.Headers.Authorization = new BasicAuthenticationHeaderValue(
                    //    this._username,
                    //    this._password);

                    httpRequestMessage.Headers.Add(
                        "Accept",
                        "*/*");

                    using (var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result)
                    {
                        try
                        {
                            httpResponseMessage.EnsureSuccessStatusCode();
                        }
                        catch
                        {
                            throw new Exception(
                                string.Concat(
                                    Convert.ToInt16(
                                        httpResponseMessage.StatusCode),
                                    " ",
                                    httpResponseMessage.ReasonPhrase));
                        }

                        return httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ExecuteGetWithAuthenticationByBearer()
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Get,
                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    httpRequestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(
                            this._mediaTypes));

                    httpRequestMessage.Headers.Add(
                        "Accept",
                        "*/*");

                    httpRequestMessage.Headers.Add(
                        "Authorization",
                        $"Bearer {this._token}");

                    using (var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result)
                    {
                        try
                        {
                            httpResponseMessage.EnsureSuccessStatusCode();
                        }
                        catch { }
                        //  {
                            //throw new Exception(
                            //    string.Concat(
                            //        Convert.ToInt16(
                            //            httpResponseMessage.StatusCode),
                            //        " ",
                            //        httpResponseMessage.ReasonPhrase));
                        //  }

                        return httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ExecutePostWithAuthenticationByBasic(string content)
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    httpRequestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(
                            this._mediaTypes));

                    httpRequestMessage.Headers.Add(
                        "Authorization",
                        $"Basic {Common.GetTokenBase64Encode(this._username, this._password)}");

                    httpRequestMessage.Headers.Add(
                        "Accept",
                        "*/*");

                    var stringContent = new StringContent(
                        content,
                        Encoding.UTF8,
                        this._mediaTypes);

                    httpRequestMessage.Content = stringContent;

                    var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result;

                    try
                    {
                        httpResponseMessage.EnsureSuccessStatusCode();
                    }
                    catch
                    {
                        throw new Exception(
                            string.Concat(
                                Convert.ToInt16(
                                    httpResponseMessage.StatusCode),
                                " ",
                                httpResponseMessage.ReasonPhrase));
                    }

                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ExecutePostWithAuthenticationByBearer(string content)
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Post,
                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    httpRequestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(
                            this._mediaTypes));

                    httpRequestMessage.Headers.Add(
                        "Authorization",
                        $"Bearer {this._token}");

                    httpRequestMessage.Headers.Add(
                        "Accept",
                        "*/*");

                    var stringContent = new StringContent(
                        content,
                        Encoding.UTF8,
                        this._mediaTypes);

                    httpRequestMessage.Content = stringContent;

                    var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result;

                    try
                    {
                        httpResponseMessage.EnsureSuccessStatusCode();
                    }
                    catch
                    {
                        throw new Exception(
                            string.Concat(
                                Convert.ToInt16(
                                    httpResponseMessage.StatusCode),
                                " ",
                                httpResponseMessage.ReasonPhrase));
                    }

                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ExecutePutWithAuthenticationByBearer(string content)
        {
            try
            {
                using (var httpRequestMessage = new HttpRequestMessage(
                    HttpMethod.Put,
                    this._requestUri))
                {
                    httpRequestMessage.Headers.Clear();

                    httpRequestMessage.Headers.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(
                            this._mediaTypes));

                    httpRequestMessage.Headers.Add(
                        "Authorization",
                        $"Bearer {this._token}");

                    httpRequestMessage.Headers.Add(
                        "Accept",
                        "*/*");

                    var stringContent = new StringContent(
                        content,
                        Encoding.UTF8,
                        this._mediaTypes);

                    httpRequestMessage.Content = stringContent;

                    var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result;

                    try
                    {
                        httpResponseMessage.EnsureSuccessStatusCode();
                    }
                    catch
                    {
                        throw new Exception(
                            string.Concat(
                                Convert.ToInt16(
                                    httpResponseMessage.StatusCode),
                                " ",
                                httpResponseMessage.ReasonPhrase));
                    }

                    return httpResponseMessage.Content.ReadAsStringAsync().Result;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ExecutePostWithoutAuthentication()
        {
            try
            {
                // Inclui o cabeçalho Accept que será enviado na requisição.
                this._httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(
                        this._mediaTypes));

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
            }
            catch
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