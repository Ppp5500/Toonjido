using System;

namespace ToonJido.Data.Model{

    public class DetailInfo{
        public MarketInfo market_info {get; set;}
        #nullable enable
        public string[]? review_contents { get; set; }
        public int[] review_grades{get; set;} = new int[]{0};
    }

    [Serializable]
    public class MarketInfo
    {
        #nullable enable
        public string find_number {get; set;} = String.Empty;
        public string market_name {get; set;} = null!;
        public string cawarock { get; set; } = String.Empty;
        public int category { get; set; }
        public string open_check { get; set; } = String.Empty;
        public string keyword_common { get; set; } = String.Empty;
        public string keyword_detail { get; set; } = String.Empty;
        public string address { get; set; } = String.Empty;
        public string phone { get; set; } = String.Empty;
        public string open_hours { get; set; } = String.Empty;
        public string item { get; set; } = String.Empty;
        public string explain { get; set; } = String.Empty;
    }
}

