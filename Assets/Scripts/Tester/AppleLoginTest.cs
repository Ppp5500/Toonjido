using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using ToonJido.Common;
using Newtonsoft.Json;
using System.IO;

public class AppleLoginTest : MonoBehaviour
{
    HttpClient client;
    // Start is called before the first frame update
    async void Start()
    {
        client = HttpClientProvider.GetHttpClient();
        string result = await APIServerLoginRequest("test", "test") ? "succes!" : "fail";
        print(result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<bool> APIServerLoginRequest(string id, string email){
        var url = appSetting.baseURL + "apple_login/";
        var values = new Dictionary<string, string>{
            {"social_login_id", "32d342dffp"},
            {"email", "secondtry"}
        };

        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);
        response.EnsureSuccessStatusCode();
        var contents = await response.Content.ReadAsStringAsync();
        
        JsonTextReader reader = new JsonTextReader(new StringReader(contents));
        while (reader.Read())
        {
            if (reader.Value != null)
            {
                print($"Token: {reader.TokenType}, Value: {reader.Value}");
            }
            else
            {
                print($"Token: {reader.TokenType}");
            }
        }

        return response.IsSuccessStatusCode;
    }
}
