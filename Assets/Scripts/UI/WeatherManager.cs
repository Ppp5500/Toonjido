using System.Net.Http;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToonJido.Control;
using ToonJido.Data.Model;
using ToonJido.Common;

namespace ToonJido.UI
{
    public class WeatherManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        private HttpClient client = HttpClientProvider.GetHttpClient();
        string wstring;
        WeatherInfo weather = new();

        public TextMeshProUGUI tem;
        public TextMeshProUGUI cloud;
        public TextMeshProUGUI hum;



        // Start is called before the first frame update
        void Start()
        {
            CurrentControl.weatherAction += DisplayWeather;
        }

        public async void DisplayWeather()
        {
            wstring = await GetWeatherData();
            wstring = wstring.Substring(2, wstring.Length - 4);
            print(wstring);
            weather = JsonConvert.DeserializeObject<WeatherInfo>(wstring);
            tem.text=weather.temp.ToString() + "°C";
            switch(weather.sky){
                case 1:
                    cloud.text = "맑은 날";
                    break;
                case 4:
                    cloud.text = "흐린 날";
                    break;
            }
            hum.text = weather.humidity + "%";

            //weatherText.text = ($"기온: {weather.temp}, 강수확률: {weather.rainfall}, 습도: {weather.humidity}");
        }

        private async Task<string> GetWeatherData()
        {
            var response = await client.GetAsync(appSetting.baseURL + "get_weather_api");
            response.EnsureSuccessStatusCode();
            string result = await response.Content.ReadAsStringAsync();
            result = Regex.Unescape($@"{result}");
            return result;
        }
    }
}

