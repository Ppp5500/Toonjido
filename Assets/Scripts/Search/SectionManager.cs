using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using static appSetting;

using TMPro;

using ToonJido.Common;
using ToonJido.Data.Model;

using UnityEngine;
using UnityEngine.UI;

namespace ToonJido.Search
{
    public class SectionManager : MonoBehaviour
    {
        public GameObject numberCanvas;
        public GameObject camTarget;
        [SerializeField] GameObject[] sections = new GameObject[15];
        [SerializeField] List<Toggle> toggles;
        TMP_Text[] sectionText = new TMP_Text[15];
        Button[] buttons = new Button[15];
        GameObject[] areaMarker = new GameObject[15];
        UnityEngine.UI.Image[] cateImages = new UnityEngine.UI.Image[15];
        UnityEngine.UI.Image[] cateBGs = new UnityEngine.UI.Image[15];
        UnityEngine.UI.Image[] cateSecondBGs = new UnityEngine.UI.Image[15];

        [Space(10)]
        [Header("Resources")]
        public Sprite[] cateSprite = new Sprite[6];
        public Sprite[] cateBGSprite = new Sprite[6];
        public Sprite[] cateSecondBGSprite = new Sprite[6];
        public Color[] cateFontColor = new Color[6];

        // temp properties
        private bool anyToggleOn = false;
        [SerializeField] private Dictionary<Toggle, int> toggleDic = new Dictionary<Toggle, int>();
        private GameObject curSelected;
        private GameObject curMarker;

        [HideInInspector] public int category;

        // common manager
        private HttpClient client = HttpClientProvider.GetHttpClient();

        private void Awake() {
            for (int i = 0; i < numberCanvas.transform.childCount; i++)
            {
                sections[i] = numberCanvas.transform.GetChild(i).gameObject;
                sectionText[i] = sections[i].GetComponentInChildren<TMP_Text>();
                buttons[i] = sections[i].GetComponent<Button>();
                var curGameObject = buttons[i].gameObject;
                int num = i;
                buttons[i].onClick.AddListener(async () => await FocusToSectionAsync(curGameObject, num));
                cateImages[i] = sections[i].transform.GetChild(2).GetComponent<UnityEngine.UI.Image>();
                cateBGs[i] = sections[i].GetComponent<UnityEngine.UI.Image>();
                cateSecondBGs[i] = sections[i].transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
                areaMarker[i] = sections[i].transform.GetChild(3).gameObject;
            }
        }

        private void Start() {
            numberCanvas.SetActive(false);

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

                curMarker = target.transform.GetChild(3).gameObject;
                curMarker.SetActive(true);
            }
            else{

                await SearchManager.instance.SearchByCategoryInSectionAsync(category, sectionNum);
            }  
        }

        // cam move method
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
            var cate = input.cultures[0].category;
            var currCateSprite = cateSprite[cate - 1];
            var currCateBGSprite = cateBGSprite[cate - 1];
            var currCateSecondBGSprite = cateSecondBGSprite[cate - 1];
            var currFontColor = cateFontColor[cate - 1];
            for(int i = 0; i < sectionText.Count(); i++){
                string propertyName = "section" + (i + 1) + "_count";

                // 리플렉션
                sectionText[i].text = input.GetType().GetProperty(propertyName).GetValue(input, null).ToString();
                // sectionText[i].color = currFontColor;
                cateImages[i].sprite = currCateSprite;
                cateBGs[i].sprite = currCateBGSprite;
                cateSecondBGs[i].sprite = currCateSecondBGSprite;
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
                numberCanvas.SetActive(true);
            }
            else{
                numberCanvas.SetActive(false);
            }
        }
    }
}

