using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SignInDevCenter
{
    public class HttpService
    {
        HttpClientHandler _handler = null;
        HttpClient _client = null;
        public HttpService()
        {
            _handler = new HttpClientHandler();
            _handler.AllowAutoRedirect = false;
            _client = new HttpClient(_handler);
        }

        public HttpService(Uri uri, string cookie):this()
        {
            _handler.CookieContainer.SetCookies(uri, cookie);
        }

        public async Task<HttpResponseMessage> SendRequest(string url, HttpMethod method, HttpContent content = null, Dictionary<string, string> appendHeaders = null)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(method, url);
                request.Content = content;
                if (appendHeaders != null)
                {
                    foreach (var header in appendHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
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

        public async Task<string> SendRequestForString(string url, HttpMethod method, HttpContent content = null, Dictionary<string, string> appendHeader = null)
        {
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await SendRequest(url, method, content, appendHeader);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return await responseMessage.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }
    }
}
