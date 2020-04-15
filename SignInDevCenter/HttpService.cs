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

        public async Task<HttpResponseMessage> SendRequest(string url, HttpContent content = null, bool isPost = false)
        {
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(isPost ? HttpMethod.Post : HttpMethod.Get, url);
                if (isPost)
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
