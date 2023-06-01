using UnityEngine;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using System.Threading.Tasks;
using System.Net.Http;
using static appSetting;

namespace ToonJido.Test.Section
{
    public class SectionManager : MonoBehaviour
    {
        public GameObject parent;
        [SerializeField] private GameObject[] sections = new GameObject[15];
        [SerializeField] private TMP_Text[] sectionText = new TMP_Text[15];
        private HttpClient client = HttpClientProvider.GetHttpClient();

        private void Awake() {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                sections[i] = parent.transform.GetChild(i).gameObject;
                sectionText[i] = sections[i].GetComponentInChildren<TMP_Text>();
            }
        }

        public async void SearchCategoryForSectionAsync(int input){
            string result = await SearchSectionInfo(input);
            SearchedStoreWithSection info = JsonConvert.DeserializeObject<SearchedStoreWithSection>(result);    
            SectionNumberUpdate(info);
        }

        private async Task<string> SearchSectionInfo(int section_num)
        {
            var searchURL = baseURL + "category_search/?query=" + section_num;
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        private void SectionNumberUpdate(SearchedStoreWithSection input){
            for(int i = 0; i < sectionText.Count(); i++){
                string propertyName = "section" + (i + 1) + "_count";
                // 리플렉션
                sectionText[i].text = input.GetType().GetProperty(propertyName).GetValue(input, null).ToString();
            }
        }
    }
}

