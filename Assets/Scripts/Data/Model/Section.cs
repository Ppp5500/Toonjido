using System;

namespace ToonJido.Test.Section
{
    [Serializable]
    public class SectionInfo
    {
        public int sectionNum { get; set; }
        public Store[] stores { get; set; }
    }

    [Serializable]
    public class Store
    {
        public string storeName { get; set; }
        public int category { get; set; }
        public string address { get; set; }
    }
}
