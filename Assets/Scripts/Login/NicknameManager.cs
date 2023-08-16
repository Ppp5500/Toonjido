using System.Collections;
using System.Collections.Generic;
using System.Net.Http;

using Newtonsoft.Json;

using static appSetting;

using ToonJido.Common;

using UnityEngine;
using Newtonsoft.Json.Linq;
using ToonJido.Login;
using TMPro;
using System.Threading.Tasks;

public class NicknameManager : MonoBehaviour
{
    public TextMeshProUGUI nicknameText;
    // common
    HttpClient client = HttpClientProvider.GetHttpClient();

    void Start()
    {
        
    }

#if DEVELOPMENT
    private void OnGUI() {
        if(GUI.Button(new Rect(500, 500, 120, 120), "post!")){
            SetDataOnServer("qwersadf");
        }

        if(GUI.Button(new Rect(500, 700, 120, 120), "get")){
            GetDataSavedOnServer();
        }
    }
#endif

    async Task InitProfileAsync(){
        var o = await GetDataSavedOnServer();

        
    }

    async Task<JObject> GetDataSavedOnServer(){
        var url = baseURL + "get_user_profile/";
        var response = await client.GetAsync(url + "?social_login_id=" + UserProfile.social_login_id);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        JObject o = JObject.Parse(result);

        return o;
    }

    async void SetDataOnServer(string _nickName){
        var url = baseURL + "save_user_info/";

        var values = new Dictionary<string, string>{
            {"account_id", UserProfile.social_login_id},
            {"nickname", _nickName},
            {"profile_img", "2"}
        };
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
    }
}
