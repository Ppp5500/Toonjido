using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using System.Threading.Tasks;
using System.Net.Http;
using ToonJido.Common;
using UnityEngine.UI;
using static appSetting;
using System.Collections.Generic;

namespace ToonJido.Search
{
    public class SectionManager : MonoBehaviour
    {
        public GameObject parent;
        public GameObject camTarget;
        [SerializeField] private GameObject[] sections = new GameObject[15];
        [SerializeField] private TMP_Text[] sectionText = new TMP_Text[15];
        [SerializeField] private Button[] buttons = new Button[15];
        [SerializeField] private GameObject[] areaMarker = new GameObject[15];
        [SerializeField] private List<Toggle> toggles;
        private bool anyToggleOn = false;
        [SerializeField] private Dictionary<Toggle, int> toggleDic = new Dictionary<Toggle, int>();
        private GameObject curSelected;
        private GameObject curMarker;
        private HttpClient client = HttpClientProvider.GetHttpClient();
        public int category;

        private void Awake() {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                sections[i] = parent.transform.GetChild(i).gameObject;
                sectionText[i] = sections[i].GetComponentInChildren<TMP_Text>();
                buttons[i] = sections[i].GetComponent<Button>();
                var curGameObject = buttons[i].gameObject;
                int num = i;
                buttons[i].onClick.AddListener(async () => await FocusToSectionAsync(curGameObject, num));
                areaMarker[i] = sections[i].transform.GetChild(1).gameObject;
            }
        }

        private void Start() {
            parent.SetActive(false);
            foreach(var item in toggles.Select((value, index) => (value, index))){
                toggleDic.Add(item.value, item.index + 1);
            }
            foreach(KeyValuePair<Toggle, int> item in toggleDic){
                item.Key.onValueChanged.AddListener( delegate{ SetCategory(item.Key, item.Value); });
            }
        }

        public async Task FocusToSectionAsync(GameObject target, int curIndex){
            var sectionNum = curIndex +1;
            if(target != curSelected){
                if(curMarker is not null){
                    curMarker.SetActive(false);
                }

                var cam = Camera.main;
                var camWidth = cam.pixelWidth;
                var camHeight = cam.pixelHeight;

                Vector3 viewPos = cam.ScreenToWorldPoint(new Vector3(camWidth/2, camHeight/3, 500));
                Vector3 centerPos = cam.ScreenToWorldPoint(new Vector3(camWidth/2, camHeight/2, 500));
                var temp = viewPos - centerPos;

                curSelected = target;

                Vector3 lot = new Vector3(target.transform.position.x, 0, target.transform.position.z + temp.z);

                StartCoroutine(FTS(lot));

                curMarker = target.transform.GetChild(1).gameObject;
                curMarker.SetActive(true);
            }
            else{

                await SearchManager.instance.SearchByCategoryInSectionAsync(category, sectionNum);
            }  
        }

        IEnumerator FTS(Vector3 target){
            float t = 0.0f;
            Vector3 startPosition = camTarget.transform.position;

            while(t < 1){
                t += Time.deltaTime/0.5f;
                camTarget.transform.position = Vector3.Lerp(startPosition, target, t);
                yield return null;
            }
        }

        public async void SearchCategoryForSectionAsync(int input){
            string result = await SearchSectionInfo(input);
            var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
            SearchedStoreWithSection info = JsonConvert.DeserializeObject<SearchedStoreWithSection>(result, setting);    
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
                // print($"sectionText[i].name: {sectionText[i].name}, sectionText[i].text: {sectionText[i].text}");
            }
        }

        public void SetCategory(Toggle _toggle, int _input){
            if(_toggle.isOn){
                category = _input;
                SearchCategoryForSectionAsync(category);
            }

            // 켜진 토글이 있는지 검사
            foreach(var item in toggles){
                if(item.isOn){
                    anyToggleOn = true;
                    break;
                }
                else{
                    anyToggleOn = false;
                }
            }

            // 켜진 토글이 있으면 섹션 캔버스 Active
            if(anyToggleOn){
                parent.SetActive(true);
            }
            else{
                parent.SetActive(false);
            }
        }
    }
}

