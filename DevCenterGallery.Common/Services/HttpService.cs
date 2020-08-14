using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;

namespace DevCenterGallary.Common.Services
{
    public class HttpService
    {
        private HttpClientHandler _handler;
        private HttpClient _client;
        
        public HttpService(bool allowAutoRedirect = true)
        {
            _handler = new HttpClientHandler();
            _handler.AllowAutoRedirect = allowAutoRedirect;
            _client = new HttpClient(_handler);
        }

        public void SetCookie(string host,string cookie)
        {
            _handler.CookieContainer.SetCookies(new Uri(host), cookie);
        }

        public async Task<Stream> SendRequestAndGetStream(string uri, HttpMethod httpMethod, HttpContent content = null)
        {
            try
            {
                var response = await SendRequest(uri, httpMethod, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                return null;
            }
            catch
            {
                throw;
            }
        }


        public async Task<string> SendRequestAndGetString(string uri, Encoding encoding, HttpMethod httpMethod, HttpContent content = null)
        {
            using (var stream = await SendRequestAndGetStream(uri, httpMethod, content))
            {
                using (var reader = new StreamReader(stream, encoding))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public async Task<HttpResponseMessage> SendRequest(string uri, HttpMethod httpMethod, HttpContent content = null)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(httpMethod, uri);
                if (httpMethod == HttpMethod.Post || httpMethod == HttpMethod.Put)
                {
                    request.Content = content;
                }
                return await _client.SendAsync(request);
            }
            catch
            {
                throw;
            }
            finally
            {
                request?.Dispose();
            }
        }
    }
}
