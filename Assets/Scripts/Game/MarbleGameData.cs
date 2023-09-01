using System.Collections.Generic;

namespace ToonJido.Data.Model{
    public class MarbleGameData
    {
        public List<Marble> marbles {get; set;}
    }

    public class Marble
    {
        public string target {get; set;}
        public string stars {get; set;}
    }
}