using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DG.Tweening;

using Newtonsoft.Json;

using TMPro;

using ToonJido.Common;
using ToonJido.Control;
using ToonJido.Data.Model;

using UnityEngine;

namespace ToonJido.UI
{
    public class WeatherManager : MonoBehaviour
    {
        // GameObjects
        [SerializeField] private GameObject rainEffect;
        [SerializeField] private Transform hTargetPos;
        [SerializeField] private Transform hFirstPos;

        private Light dirLight;

        [Header("Weather Display UI")]
        public UnityEngine.UI.Image todayWeathericon;
        public TextMeshProUGUI weekdayText;
        public TextMeshProUGUI todayMaxTem;
        public TextMeshProUGUI todayMinTem;
        public TextMeshProUGUI tem;
        public TextMeshProUGUI cloud;
        public UnityEngine.UI.Image dustPanel;
        public UnityEngine.UI.Image dustIcon;

        [Space(10)]
        public List<Forecast> forecastuis = new List<Forecast>();

        [Space(10)]
        public TextMeshProUGUI hum;
        public TextMeshProUGUI dust;

        [Header("Skybox Materials")]
        public Material clearSkybox;
        public Material cloudySkybox;
        public Cubemap cloudyCubemap;

        [Header("Sky Color properties")]
        private readonly Color clearSkyDirLightColor = new Color(255, 244, 214, 255);
        private readonly Color cloudySkyDirLightColor = new Color(255, 244, 214, 255);

        [ColorUsage(true, true)]
        public Color clearSkyAmientColor;
        [ColorUsage(true, true)]
        public Color cloudySkyAmientColor;

        [Header("Today Weather Icon Sprites")]
        public Sprite clearIcon;
        public Sprite cloudyIcon;
        public Sprite rainyIcon;
        public Sprite snowyIcon;

        [Header("Sky Icon Sprites")]
        public Sprite clearSky;
        public Sprite cloudySky;
        public Sprite rainysky;
        public Sprite thunderSky;
        public Sprite snowySky;

        [Header("Dust Panel Sprites")]
        public Sprite goodDust;
        public Sprite normalDust;
        public Sprite badDust;
        public Sprite veryBadDust;

        [Header("Dust Icon Sprites")]
        public Sprite goodDustIcon;
        public Sprite normalDustIcon;
        public Sprite badDustIcon;
        public Sprite veryBadDustIcon;

        // common managers
        private HttpClient client = HttpClientProvider.GetHttpClient();
        public SevenStarMarbleGameManager ssmgManager;

        // Start is called before the first frame update
        async void Start()
        {
            CurrentControl.weatherAction += DisplayWeather;
            dirLight = GameObject.Find("Directional Light").GetComponent<Light>();

            await CheckTodayWeather();
        }

        public async void DisplayWeather()
        {
            ssmgManager.FlyingAround();
            weekdayText.text = GetWeekday();
            WeatherInfo weather = await GetWeatherData();
            var forecasts = await GetWeatherForecast();
            var dustData = await GetDustData();

            todayWeathericon.sprite = weather.rainType switch
            {
                0 => clearIcon,
                1 => rainyIcon,
                2 => snowyIcon,
                3 => snowyIcon,
                5 => rainyIcon,
                6 => rainyIcon,
                7 => snowyIcon,
                _ => clearIcon
            };

            // 비는 오지 않지만, 흐릴 때 아이콘 처리
            if(weather.rainType == 0 && weather.sky == 3) { todayWeathericon.sprite = cloudyIcon; }
            if(weather.rainType == 0 && weather.sky == 4) { todayWeathericon.sprite = cloudyIcon; }

            tem.text=weather.temp.ToString() + "°C";
            todayMinTem.text = forecasts[0].TMN;
            todayMaxTem.text = forecasts[0].TMX;

            cloud.text = weather.sky switch
            {
                1 => "맑음",
                3 => "흐림",
                4 => "흐림",
                _ => "맑음",
            };

            for (int i = 0; i < forecastuis.Count; i++)
            {
                forecastuis[i].minTem.text = forecasts[i].TMN;
                forecastuis[i].maxTem.text = forecasts[i].TMX;
                forecastuis[i].sky.sprite = GetPTY(forecasts[i].PTY);                
            }

            hum.text = weather.humidity + "%";
            dust.text = dustData.pm10Grade;
            switch(dustData.pm10Grade){
                case "좋음":
                    dustPanel.sprite = goodDust;
                    break;
                case "보통":
                    dustPanel.sprite = normalDust;
                    break;
                case "나쁨":
                    dustPanel.sprite = badDust;
                    break;
                case "매우나쁨":
                    dustPanel.sprite = veryBadDust;
                    break;
            }

            dustIcon.sprite = dustData.pm10Grade switch
            {
                "좋음" => goodDustIcon,
                "보통" => normalDustIcon,
                "나쁨" => badDustIcon,
                "매우나쁨" => veryBadDustIcon,
                _ => normalDustIcon
            };
        }

        public Sprite GetPTY(string input)
        {
            return input switch
            {
                "0" => clearSky,
                "1" => rainysky,
                "2" => rainysky,
                "3" => snowySky,
                "4" => thunderSky,
                _ => clearSky
            };
        }

        private string GetWeekday()
        {
            return DateTime.Now.DayOfWeek switch
            {
                DayOfWeek.Monday => "Monday",
                DayOfWeek.Tuesday => "Tuesday",
                DayOfWeek.Wednesday => "Wednesday",
                DayOfWeek.Thursday => "Thursday",
                DayOfWeek.Friday => "Friday",
                DayOfWeek.Saturday => "Saturday",
                DayOfWeek.Sunday => "Sunday",
                _ => "Today"
            };
        }

        private async Task<WeatherInfo> GetWeatherData()
        {
            var response = await client.GetAsync(appSetting.baseURL + "get_weather_api");
            response.EnsureSuccessStatusCode();
            string searchResult = await response.Content.ReadAsStringAsync();
            searchResult = Regex.Unescape($@"{searchResult}");
            searchResult = searchResult.Substring(2, searchResult.Length - 4);
            WeatherInfo weather = JsonConvert.DeserializeObject<WeatherInfo>(searchResult);

            return weather;
        }

        private async Task<List<Weather>> GetWeatherForecast(){
            var response = await client.GetAsync(appSetting.baseURL + "get_temperature/");
            response.EnsureSuccessStatusCode();
            string searchResult = await response.Content.ReadAsStringAsync();
            string json = JsonConvert.DeserializeObject<WeatherDataParent>(searchResult).weather_data;
            List<Weather> weatherList = JsonConvert.DeserializeObject<List<Weather>>(json);

            return weatherList;
        }

        private async Task<DustData> GetDustData(){
            var response = await client.GetAsync(appSetting.baseURL + "get_fineDust/");
            response.EnsureSuccessStatusCode();
            string searchResult = await response.Content.ReadAsStringAsync();
            searchResult = Regex.Unescape($@"{searchResult}");
            searchResult = searchResult.Substring(2, searchResult.Length - 4);
            DustData dustData = JsonConvert.DeserializeObject<DustData>(searchResult);

            return dustData;
        }

        private async Task CheckTodayWeather(){
            var weather = await GetWeatherData();
            switch(weather.sky){
                case 1:
                    rainEffect.SetActive(false);
                    ChangeClearSky();
                    break;
                case 4:
                    rainEffect.SetActive(true);
                    ChangeCloudySky();
                    RenderSettings.customReflection = cloudyCubemap;
                    break;
            }
        }

        public void ChangeClearSky(){
            RenderSettings.skybox = clearSkybox;
            RenderSettings.ambientLight = clearSkyAmientColor;
            //dirLight.color = clearSkyDirLightColor;
        }

        public void ChangeCloudySky(){
            RenderSettings.skybox = cloudySkybox;
            RenderSettings.ambientLight = cloudySkyAmientColor;
            //dirLight.color = cloudySkyDirLightColor;
            dirLight.intensity = 0.5f;
        }
    }

    [System.Serializable]
    public struct Forecast{
        public UnityEngine.UI.Image sky;
        public TextMeshProUGUI minTem;
        public TextMeshProUGUI maxTem;
    }
}

