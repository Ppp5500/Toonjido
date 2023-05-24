using System.Net.Http;

public static class HttpClientProvider
{
    private static HttpClient httpClient = new();
    public static HttpClient GetHttpClient(){
        return httpClient;
    }
}
