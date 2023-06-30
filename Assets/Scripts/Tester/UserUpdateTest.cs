using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using ToonJido.Common;
using System.Threading.Tasks;

public class UserUpdateTest : MonoBehaviour
{
    HttpClient client = HttpClientProvider.GetHttpClient();
    // Start is called before the first frame update
    async void Start()
    {
        //await postuserinfo();
        await getUserinfo();
    }

    public async Task postuserinfo(){
        var values = new Dictionary<string, string>{
            {"social_login_id", "2774886049"},
            {"email", "pinkaulait@daum.net"},
            {"nickname", "test01"}
        };

        string url = appSetting.baseURL + "save_user_info/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }

    public async Task getUserinfo(){
        var values = new Dictionary<string, string>{
            {"social_login_id", "2774886049"},
        };

        string url = appSetting.baseURL + "save_user_info/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }

    public async Task DeleteZzim(string _marketId){
        var values = new Dictionary<string, string>{
            {"account", "2774886049"},
            {"market_id", _marketId}
        };

        string url = appSetting.baseURL + "delete_favorite/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        print(response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
    }
}
