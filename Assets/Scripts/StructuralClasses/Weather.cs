
using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class WeatherInfo
    {
        public int id {  get; set; }
        public string timestamp { get; set; }
        public float temp { get; set; }
        public float humidity { get; set; }
        public int rainType { get; set; }
        public string rainfall { get; set; }
        public int sky { get; set; }
    }
}

