using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ToonJido.Common;
using UnityEngine;


public class zzimTest : MonoBehaviour
{
    HttpClient client = HttpClientProvider.GetHttpClient();
    // Start is called before the first frame update
    void Start()
    {
        postzzim();
        //getzzim();
        //await DeleteZzim("236");

        //getzzim();
    }

    public async void postzzim(){
        var values = new Dictionary<string, string>{
            {"account", "2774886049"},
            {"market_id", "236"},
            {"is_favorite", "True"}
        };

        string url = appSetting.baseURL + "create_favorite/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }

    public async void getzzim(){
        string url = "http://43.201.215.208:8000/hongbo/get_favorite_market_ids/" + "?social_login_id=2774886049";

        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
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
