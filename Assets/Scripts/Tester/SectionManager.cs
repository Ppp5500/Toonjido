using UnityEngine;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using ToonJido.Data.Model;

namespace ToonJido.Test.Section
{
    public class SectionManager : MonoBehaviour
    {
        public GameObject parent;
        public GameObject test;
        public TextAsset jsonFile;
        [SerializeField] private GameObject[] sections = new GameObject[15];
        [SerializeField] private TMP_Text[] sectionText = new TMP_Text[15];
        SectionInfo sec15 = new();


        private void Start()
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                sections[i] = parent.transform.GetChild(i).gameObject;
                sectionText[i] = sections[i].GetComponentInChildren<TMP_Text>();
            }

            sectionText[0].text = "asdf";

            jsonFile = Resources.Load<TextAsset>("Section15");

            sec15 = JsonConvert.DeserializeObject<SectionInfo>(jsonFile.ToString());
        }

        public void SelectCategory(int category)
        {
            var test = from store in sec15.stores
                       where store.category == category
                       select store;
            sectionText[0].text = test.Count().ToString();
        }

        public void Selectthis(int category)
        {
            var test = from store in sec15.stores
                       where store.category == category
                       select store;
            sectionText[0].text = test.Count().ToString();
        }
    }
}

