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
        public int id { get; set; }
        public string lot_number { get; set; } = String.Empty;
        public string market_name { get; set; } = null!;
        public string cawarock {get; set;} = String.Empty;
        public int category{get; set;}
        public string floor { get; set; } = String.Empty;
        public string open_check { get; set; } = String.Empty;
        public string keyword_common { get; set;} = String.Empty;
        public string keyword_detail { get; set;} = String.Empty;
        public string address { get; set;} = String.Empty;
        public string phone { get; set;} = String.Empty;
        public string open_hours { get; set;} = String.Empty;
        public string item {get; set;} = String.Empty;
        public string explain { get; set; } = String.Empty;
        public string section { get; set; } = String.Empty;
        public review[]? reviews { get; set; }
        public Image[]? images { get; set;}
    }

    [Serializable]
    public class review{
        public string title { get; set; } = string.Empty;
        public string content {get; set; } = string.Empty;
        public int rating { get; set; } = 0;
    }

    [Serializable]
    public class Image{
        public int market_id { get; set; }
        public string image {get; set; } = string.Empty;
    }
}

