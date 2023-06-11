using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using ToonJido.Common;
using static appSetting;

namespace ToonJido.Test
{
    public class PostTest : MonoBehaviour
    {
        HttpClient client = HttpClientProvider.GetHttpClient();

        // Start is called before the first frame update
        void Start() {
            MyRequest();
         }

        // Update is called once per frame
        void Update() { }

        public async void MyRequest()
        {
            var values = new Dictionary<string, string>
            {
                { "social_login_id", "4" },
                { "email", "pinkaulait@" }
            };
            string url = baseURL + "save_user_info/";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data);
            var contents = response.Content.ReadAsStringAsync();
            print($"contents: {contents}");
        }
    }
}
