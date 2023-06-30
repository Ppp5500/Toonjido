using System;

namespace ToonJido.Data.Model{
    [Serializable]
    public class WeatherDataParent
    {
        public string weather_data{get; set;}
    }

    [Serializable]
    public class Weather{
        public string date{get; set;}
        public string SKY{get; set;}
        public string PTY{get; set;}
        public string TMN{get; set;}
        public string TMX{get; set;}
    }
}

