using System;

namespace ToonJido.Data.Model
{
    [Serializable]
    public class SearchedStore
    {
#nullable enable
        public Culture[]? cultures { get; set; }
        public int section1_count {get; set;} = 0;
        public int section2_count {get; set;} = 0;
        public int section3_count {get; set;} = 0;
        public int section4_count {get; set;} = 0;
        public int section5_count {get; set;} = 0;
        public int section6_count {get; set;} = 0;
        public int section7_count {get; set;} = 0;
        public int section8_count {get; set;} = 0;
        public int section9_count {get; set;} = 0;
        public int section10_count {get; set;} = 0;
        public int section11_count {get; set;} = 0;
        public int section12_count {get; set;} = 0;
        public int section13_count {get; set;} = 0;
        public int section14_count {get; set;} = 0;
        public int section15_count {get; set;} = 0;
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
        public string img { get; set; } = String.Empty;
        public string keyword_common { get; set;} = String.Empty;
        public string keyword_detail { get; set;} = String.Empty;
        public string address { get; set;} = String.Empty;
        public string phone { get; set;} = String.Empty;
        public string open_hours { get; set;} = String.Empty;
        public string item {get; set;} = String.Empty;
        public string explain { get; set; } = String.Empty;
        public string section { get; set; } = String.Empty;
        public review review { get; set; } = new();
        public grade grade { get; set;} = new();
    }

    [Serializable]
    public class review{
        public string title { get; set; } = string.Empty;
        public string content {get; set; } = string.Empty;
        public int rating { get; set; } = 0;
    }

    [Serializable]
    public class grade{
        public string title { get; set; } = string.Empty;
        public string content {get; set; } = string.Empty;
        public int rating { get; set; } = 0;
    }
}

