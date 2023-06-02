using System.Net.Http;

namespace ToonJido.Common
{
    public static class HttpClientProvider
    {
        private static HttpClient httpClient = new();

        public static HttpClient GetHttpClient()
        {
            return httpClient;
        }
    }
}
