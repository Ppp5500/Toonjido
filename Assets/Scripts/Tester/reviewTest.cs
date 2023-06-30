using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using ToonJido.Common;
using System.Threading.Tasks;
using ToonJido.Data.Model;
using System;
using Newtonsoft.Json;

public class reviewTest : MonoBehaviour
{
    HttpClient client = HttpClientProvider.GetHttpClient();
    // Start is called before the first frame update
    async void Start()
    {
        //await postReview();
        //await getReview();
        //await deleteReivew("236");
        await GetDetailReview();
    }

    public async Task postReview(){
        var values = new Dictionary<string, string>{
            {"account", "2774886049"},
            {"market_id", "236"},
            {"content", "테스트용 맛있어요!"},
            {"grade", "5"}
        };

        string url = appSetting.baseURL + "create_review/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }

    public async Task getReview(){
        string url = appSetting.baseURL + "get_review_market_ids/" + "?social_login_id="+"2774886049";
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }

    public async Task deleteReivew(string _marketId){
        var values = new Dictionary<string, string>{
            {"account", "2774886049"},
            {"market_id", _marketId}
        };

        string url = appSetting.baseURL + "delete_review/";
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        print(response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
    }

    public async Task GetDetailReview(){
        string url = appSetting.baseURL + "get_favorite_review/" + "?social_login_id=" + "2774886049" + "&market_id=" + "236";
        print(url);
        var response = await client.GetAsync(url);

        response.EnsureSuccessStatusCode();
        var searchResult = await response.Content.ReadAsStringAsync();
        print(searchResult);
        
        // 내보낼 객체 생성
        DetailInfo result = new();
        var setting = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };
        result = JsonConvert.DeserializeObject<DetailInfo>( searchResult, setting );
        print(response.ToString());
        print(result.market_info.market_name);
    }
}
