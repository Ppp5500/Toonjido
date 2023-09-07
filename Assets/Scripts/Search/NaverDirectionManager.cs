using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using ToonJido.Common;

using UnityEngine;

namespace ToonJido.Search{
    public class NaverDirectionManager : MonoBehaviour
    {
        private HttpClient client;

        // Start is called before the first frame update
        void Start()
        {
            client = HttpClientProvider.GetHttpClient();
        }

        async public Task<JToken[]> GetPathFromNaver(Vector3 _startPoint, Vector3 _goalPoint){
            var startPoint = GPSEncoder.USCToGPS(_startPoint);
            var goalPoint = GPSEncoder.USCToGPS(_goalPoint);

            var url = "https://naveropenapi.apigw.ntruss.com/map-direction/v1/driving?start=" 
                        + startPoint.y + ","
                        + startPoint.x + "&goal="
                        + goalPoint.y + ","
                        + goalPoint.x + "&option=trafast";
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-NCP-APIGW-API-KEY-ID", appSetting.naverAPIID);
            request.Headers.Add("X-NCP-APIGW-API-KEY", appSetting.naverAPIKey);
            var response = await client.SendAsync(request);

            if(response.IsSuccessStatusCode){
                // 응답 성공 시 처리
                var data = await response.Content.ReadAsStringAsync();
                var temp = JObject.Parse(data);

                var result = temp["route"]["trafast"][0]["path"].ToArray();
                return result;
            }
            else{
                // 응답 실패 시 처리
                return null;
            }
        }
    }
}

