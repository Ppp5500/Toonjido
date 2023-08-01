using System.Collections.Generic;

namespace ToonJido.Data.Model{
    public class MarbleGameData
    {
        public List<Ball> balls {get; set;}
    }

    public class Ball
    {
        public string target {get; set;}
        public string stars {get; set;}
    }
}