using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class SearchedStore
    {
#nullable enable
        public Culture[]? cultures { get; set; }
    }

    [Serializable]
    public class Culture
    {
#nullable enable
        public int id { get; set; }
        public int idx { get; set; }
        public string name { get; set; } = null!;
        public string? explanation { get; set; }
        public int? grade { get; set; }
        public string? review { get; set; }
        public string? main_item { get; set; }
        public string? market_hours { get; set; }
        public string? phone_number { get; set; }
        public string? address { get; set; }
        public string? category { get; set; }
        public int? section_number { get; set; }
        public byte[]? market_img { get; set; }
    }
}

