namespace PayCheck.Web
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    public class WebApiHelper : IDisposable
    {
        private readonly HttpClient _httpClient;

        private readonly string _requestUri;
        private readonly string _token;

        public readonly string _mediaTypes = @"application/json";

        private bool _disposedValue = false;

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
                        var details = httpResponseMessage.Content.ReadAsStringAsync().Result;

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
                                    httpResponseMessage.ReasonPhrase,
                                    " ",
                                    details));
                        }

                        return details;
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

                    httpRequestMessage.Content = new StringContent(
                        content,
                        Encoding.UTF8,
                        this._mediaTypes);

                    using (var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result)
                    {
                        var details = httpResponseMessage.Content.ReadAsStringAsync().Result;

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
                                    " | ",
                                    httpResponseMessage.ReasonPhrase,
                                    " | ",
                                    details));
                        }

                        return details;
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

                    using (var httpResponseMessage = this._httpClient.SendAsync(
                        httpRequestMessage).Result)
                    {
                        var details = httpResponseMessage.Content.ReadAsStringAsync().Result;

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
                                    " | ",
                                    httpResponseMessage.ReasonPhrase,
                                    " | ",
                                    details));
                        }

                        return details;
                    }
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