using System.Net.Http;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ToonJido.Control;
using ToonJido.Data.Model;

namespace ToonJido.UI
{
    public class WeatherManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI weatherText;
        string wstring;
        WeatherInfo weather1 = new();


        // Start is called before the first frame update
        void Start()
        {
            CurrentControl.weatherAction += DisplayWeather;
        }

        public async void DisplayWeather()
        {
            wstring = await GetWeatherData();
            // 현재 서버측에서 보내는 json이 잘못된 파일임
            // weather1 = JsonConvert.DeserializeObject<WeatherInfo>(wstring);
            weatherText.text = wstring;
        }

        private async Task<string> GetWeatherData()
        {
            using (HttpClient client = new())
            {
                var response = await client.GetAsync(appSetting.baseURL + "get_weather_api");
                response.EnsureSuccessStatusCode();
                string result = await response.Content.ReadAsStringAsync();
                result = Regex.Unescape($@"{result}");
                return result;
            }
        }
    }
}

