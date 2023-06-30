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
        // GameObjects
        [SerializeField] private GameObject rainEffect;
        private Light dirLight;

        [Header("Weather Display UI")]
        public TextMeshProUGUI weekdayText;
        public TextMeshProUGUI todayMaxTem;
        public TextMeshProUGUI todayMinTem;
        public TextMeshProUGUI tem;
        public TextMeshProUGUI cloud;
        public TextMeshProUGUI hum;

        [Header("Skybox Materials")]
        public Material clearSkybox;
        public Material cloudySkybox;

        [Header("Sky Color properties")]
        private readonly Color clearSkyDirLightColor = new Color(255, 244, 214, 255);
        private readonly Color cloudySkyDirLightColor = new Color(255, 244, 214, 255);

        [ColorUsage(true, true)]
        public Color clearSkyAmientColor;
        [ColorUsage(true, true)]
        public Color cloudySkyAmientColor;

        // common managers
        private HttpClient client = HttpClientProvider.GetHttpClient();

        // Start is called before the first frame update
        async void Start()
        {
            CurrentControl.weatherAction += DisplayWeather;
            dirLight = GameObject.Find("Directional Light").GetComponent<Light>();

            await CheckTodayWeather();
            await GetWeatherForecast();
        }

        public async void DisplayWeather()
        {
            WeatherInfo weather = await GetWeatherData();
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
        }

        private async Task<WeatherInfo> GetWeatherData()
        {
            var response = await client.GetAsync(appSetting.baseURL + "get_weather_api");
            response.EnsureSuccessStatusCode();
            string searchResult = await response.Content.ReadAsStringAsync();
            searchResult = Regex.Unescape($@"{searchResult}");
            string wstring = searchResult.Substring(2, searchResult.Length - 4);
            var weather = JsonConvert.DeserializeObject<WeatherInfo>(wstring);
            return weather;
        }

        private async Task GetWeatherForecast(){
            var response = await client.GetAsync(appSetting.baseURL + "get_temperature/");
            response.EnsureSuccessStatusCode();
            string searchResult = await response.Content.ReadAsStringAsync();
            string json = JsonConvert.DeserializeObject<WeatherDataParent>(searchResult).weather_data;
            Weather[] weatherList = JsonConvert.DeserializeObject<Weather[]>(json);

            print(weatherList[0].date);
        }

        private async Task CheckTodayWeather(){
            var weather = await GetWeatherData();
            //switch(4){
            switch(weather.sky){
                case 1:
                    rainEffect.SetActive(false);
                    ChangeClearSky();
                    break;
                case 4:
                    rainEffect.SetActive(true);
                    ChangeCloudySky();
                    break;
            }
        }

        public void ChangeClearSky(){
            RenderSettings.skybox = clearSkybox;
            RenderSettings.ambientLight = clearSkyAmientColor;
            dirLight.color = clearSkyDirLightColor;
        }

        public void ChangeCloudySky(){
            RenderSettings.skybox = cloudySkybox;
            RenderSettings.ambientLight = cloudySkyAmientColor;
            //dirLight.color = cloudySkyDirLightColor;
            dirLight.intensity = 0.5f;
        }
    }
}

