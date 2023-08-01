using System;

namespace ToonJido.Data.Model{
    public class Fortune{
        public string num {get; set;}
        public string desc {get; set;}

        public Fortune(string _num, string _desc){
            num = _num;
            desc = _desc;
        }

        public override string ToString()
        {
            return desc;
        }
    }
}
