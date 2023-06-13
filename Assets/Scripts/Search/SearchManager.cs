using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using TMPro;
using ToonJido.Control;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using ToonJido.Common;
using ToonJido.UI;
using static appSetting;
using System.Collections;
using System;

namespace ToonJido.Search
{
    public class SearchManager : MonoBehaviour
    {
        SearchedStore storeData = new();

        [SerializeField] private UIDrag drag;
        [SerializeField] private Button searchButton;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button findRoadButton;
        [SerializeField] private GameObject searchList;
        [SerializeField] private GameObject resultScroll;
        [SerializeField] private GameObject loadingText;
        [SerializeField] private GameObject resultParent;
        [SerializeField] private GameObject resultPref;
        [SerializeField] private GameObject noResultText;
        [SerializeField] private GameObject camTarget;

#region DetailCanvasProperties
        [Header("Detail Content Scroll Properties")]
        [SerializeField] private GameObject detailContentScroll;
        [SerializeField] private Button detailContentBackButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private UnityEngine.UI.Image Thumbnail;
        [SerializeField] private TextMeshProUGUI storeName;
        [SerializeField] private Button pathFindButton;
        [SerializeField] private Button zzimButton;
        [SerializeField] private TextMeshProUGUI starText;
        [SerializeField] private TextMeshProUGUI clockText;
        [SerializeField] private TextMeshProUGUI explainText;
        [SerializeField] private TextMeshProUGUI addressText;
        [SerializeField] private TextMeshProUGUI contactText;
        [SerializeField] private UnityEngine.UI.Image[] stars = new UnityEngine.UI.Image[5];
        [SerializeField] private Sprite blankStar;
        [SerializeField] private Sprite fullStar;
        [SerializeField] private Sprite blankHeart;
        [SerializeField] private Sprite fullHeart;
#endregion DetailCanvasProperties

        private HttpClient client = HttpClientProvider.GetHttpClient();
        private List<GameObject> resultPrefs = new List<GameObject>();
        public static SearchManager instance;

        private async void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            try
            {
                using (StreamReader reader = new(dataPath))
                {
                    var data = await reader.ReadToEndAsync();
                    var setting = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };

                    storeData = JsonConvert.DeserializeObject<SearchedStore>(data, setting);
                }
            }
            catch (FileNotFoundException ex)
            {
                print(ex.Message);
            }
        }

        void Start()
        {
            drag = searchList.GetComponent<UIDrag>();
            searchButton.onClick.AddListener(() => ClickSearchButton());
            detailContentBackButton.onClick.AddListener(() => BackToSearchResult());
            findRoadButton.onClick.AddListener(() => inputField.ActivateInputField());
        }

        public async void ClickSearchButton()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                // CurrentControl.ChangeToSearchResult();
                noResultText.SetActive(false);
                var searchResult = await SearchStore(inputField.text);
                SearchedStore searchResultArr = new();
                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                searchResultArr = JsonConvert.DeserializeObject<SearchedStore>(
                    searchResult,
                    setting
                );

                if (searchResultArr.cultures.Length == 0)
                {
                    noResultText.SetActive(true);
                }
                else
                {
                    await DisplayResult(searchResultArr);
                }
            }
            else
                return;
        }

        private async Task<string> SearchStore(string keyword)
        {
            var searchURL = baseURL + "search/?query=" + keyword;
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public SearchedStore SearchStoreByAddress(string address)
        {
            // var searchURL = baseURL + "get_Market_DB_List";
            // var response = await client.GetAsync(searchURL);
            // response.EnsureSuccessStatusCode();
            // var httpResult = await response.Content.ReadAsStringAsync();
            //SearchedStore searchedStore = new();
            // var setting = new JsonSerializerSettings
            // {
            //     NullValueHandling = NullValueHandling.Ignore,
            //     MissingMemberHandling = MissingMemberHandling.Ignore
            // };
            // searchedStore = JsonConvert.DeserializeObject<SearchedStore>(httpResult, setting);

            SearchedStore searchedStore = new();
            searchedStore.cultures = storeData.cultures
                .Where(x => x.find_number == address)
                .ToArray();
            return searchedStore;
        }

        public async Task SearchByCategoryInSectionAsync(int category, int section){
            SearchedStore searchedStore = new();
            string sumSection = "Section" + section;
            searchedStore.cultures = storeData.cultures
                .Where(x => x.section == sumSection)
                .Where(x => x.category == category)
                .ToArray();
            await DisplayResult(searchedStore);
        }

        public async void SearchKeyStoresAsync(){
            SearchedStore tempSearchedStore = new();
            //Culture[] tempCultures = new Culture[15];
            SearchedStore result = new(){
                cultures = new Culture[13]
            };

            string[] keyStoresName = {
                "천안요리학원",
                "청담동5번가",
                "대호갈비",
                "역전시장 상인회",
                "마늘 떡볶이",
                "흥흥발전소",
                "황가네 뒷고기",
                "크크필름",
                "다목적 강의실",
                "명동사인회",
                "모유수유실",
                //"공유주방",
                //"빛나라청춘",
                "스마일 오락실",
                "투데이 이즈 유어 벌스데이"
            };

            for(int i = 0; i < keyStoresName.Length; i++){
                string temp = await SearchStore(keyStoresName[i]);
                var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                tempSearchedStore = JsonConvert.DeserializeObject<SearchedStore>(temp, setting);
                result.cultures[i] = tempSearchedStore.cultures[0];
            }

            await DisplayResult(result);
        }

        private SearchedStore SearchByMarketId(int[] idArr){
            SearchedStore searchedStore = new(){
                cultures = new Culture[idArr.Length]
            };
            int index = 0;
            foreach(var item in storeData.cultures){
                if(idArr.Contains(item.id)){
                    searchedStore.cultures[index] = item;
                    index++;
                }
            }
            return searchedStore;
        }

        public async Task DisplayResult(SearchedStore input)
        {
            ClearResultList();
            drag.Up();
            loadingText.SetActive(true);
            for (int i = 0; i < input.cultures.Length; i++)
            {
                GameObject mypref = Instantiate(resultPref, resultParent.transform);
                if (input.cultures[i].cawarock == string.Empty)
                {
                    mypref.transform
                        .Find("Store Name")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].market_name);
                    mypref.transform
                        .Find("explanation")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].explain);
                    mypref.GetComponent<UnityEngine.UI.Image>().color = new Color(
                        0.5f,
                        0.5f,
                        0.5f,
                        1f
                    );
                    resultPrefs.Add(mypref);

                    Button button = mypref.GetComponent<Button>();
                }
                else
                {
                    // 썸네일 이미지 다운
                    string imgURL = input.cultures[i].images[0].image;
                    var data = await client.GetByteArrayAsync(baseURL + imgURL.Substring(1));
                    Texture2D tex = new(120, 120);
                    tex.LoadImage(data, false);
                    Rect rect = new Rect(0, 0, tex.width, tex.height);
                    Sprite sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));

                    UnityEngine.UI.Image[] stars = new UnityEngine.UI.Image[5];

                    string storeName = input.cultures[i].market_name;
                    int rank = Mathf.RoundToInt(input.cultures[i].average_grade);
                    if(rank == 0) rank = 1;

                    mypref.transform
                        .Find("Main Image Mask")
                        .gameObject
                        .transform
                        .Find("Main Image")
                        .GetComponent<UnityEngine.UI.Image>()
                        .sprite = sp;
                    mypref.transform
                        .Find("Store Name")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].market_name);
                    mypref.transform
                        .Find("explanation")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].explain);
                    mypref.transform
                        .Find("Rank")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].average_grade.ToString("F1"));
                    stars = mypref.transform
                        .Find("Stars")
                        .GetComponentsInChildren<UnityEngine.UI.Image>();
                    for(int j = 0; j < rank; j++){
                        stars[j].sprite = fullStar;
                    }
                    for(int k = rank; k < 5; k++){
                        stars[k].sprite = blankStar;
                    }
                    mypref.transform
                        .Find("Bottom")
                        .gameObject
                        .transform
                        .Find("Tel Info")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].phone.Substring(2));
                    mypref.transform
                        .Find("Review Info")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText("| 리뷰 " + input.cultures[i].reviews.Length);
                    mypref.transform.SetSiblingIndex(0);

                    Button button = mypref.GetComponent<Button>();
                    button.onClick.AddListener(() => OpenDetailCanvas(storeName));
                    resultPrefs.Insert(0, mypref);
                }
            }
            loadingText.SetActive(false);
        }

        public void PathFind(string address)
        {

            Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(address);
            NavPlayerControl.navPlayerControl.SetDestination(lot);

            // if(CurrentControl.gpsStatus == GPSStatus.avaliable){
            //     Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(address);
            //     NavPlayerControl.navPlayerControl.SetDestination(lot);
            // }
            // else{
            //     NoticeManager.instance.ShowNotice("GPS를 이용할 수 없을 때는 길찾기를 할 수 없습니다.");
            // }
        }

        public void FocusToBuilding(string address){
            Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(address);
            StartCoroutine(FTB(lot));
        }

        IEnumerator FTB(Vector3 target){
            float t = 0.0f;
            Vector3 startPosition = camTarget.transform.position;

            while(t < 1){
                t += Time.deltaTime/0.5f;
                camTarget.transform.position = Vector3.Lerp(startPosition, target, t);
                yield return null;
            }
        }

        private async void OpenDetailCanvas(string name)
        {
            pathFindButton.onClick.RemoveAllListeners();
            // show detailCanvas
            detailContentScroll.SetActive(true);
            loadingPanel.SetActive(true);

            resultScroll.SetActive(false);

            // Get data
            var result = await SearchStore(name);
            var setting = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
            SearchedStore store = JsonConvert.DeserializeObject<SearchedStore>(result, setting);
            Culture currentStore = store.cultures[0];

            var market_id = currentStore.id;
            // 찜 목록을 다운 받아서 이미 찜한 상점인지 검사
            var zzimArr = await RequestZzimArray(UserProfile.social_login_id);
            bool isZzim = zzimArr.Contains(market_id);

            // 이미지 다운로드
            string imgURL = currentStore.images[0].image;
                    var data = await client.GetByteArrayAsync(baseURL + imgURL.Substring(1));
                    Texture2D tex = new(690, 500);
                    tex.LoadImage(data, false);
                    Rect rect = new Rect(0, 0, tex.width, tex.height);
                    Sprite sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));
            // 점수 계산
            int rank = Mathf.RoundToInt(currentStore.average_grade);
            if(rank == 0) rank = 1;
 

            var find_number = currentStore.find_number;
            Thumbnail.sprite = sp;
            storeName.text = currentStore.market_name;
            starText.text = currentStore.average_grade.ToString();
            explainText.text = currentStore.explain;
            clockText.text = currentStore.open_hours;
            addressText.text = currentStore.address;
            contactText.text = currentStore.phone;
            for(int j = 0; j < rank; j++){
                    stars[j].sprite = fullStar;
                    print($"{j}st star is full star!");
                }
                for(int k = rank; k < 5; k++){
                    stars[k].sprite = blankStar;
                    print($"{k}st star is blank star!");
                }
            pathFindButton.onClick.AddListener(() => PathFind(find_number));

            // 이미 찜을 한 가게면
            if(isZzim){
                // 버튼 이미지 변경
                UnityEngine.UI.Image zzimbuttonImage = zzimButton.gameObject.GetComponent<UnityEngine.UI.Image>();
                zzimbuttonImage.sprite = fullHeart;
                // 버튼에 리스너 추가
                zzimButton.onClick.AddListener(async () => await DeleteZzim(UserProfile.social_login_id, market_id.ToString(), zzimButton));
            }
            // 찜을 안 한 가게면
            else{
                // 버튼 이미지 변경
                UnityEngine.UI.Image zzimbuttonImage = zzimButton.gameObject.GetComponent<UnityEngine.UI.Image>();
                zzimbuttonImage.sprite = blankHeart;
                // 버튼에 리스너 추가
                zzimButton.onClick.AddListener(async () => await PostZzim(UserProfile.social_login_id, market_id.ToString(), true, zzimButton));
            }

            menuButton.gameObject.SetActive(false);
            detailContentBackButton.gameObject.SetActive(true);
            loadingPanel.SetActive(false);
        }

        private void BackToSearchResult(){
            resultScroll.SetActive(true);
            detailContentScroll.SetActive(false);

            menuButton.gameObject.SetActive(true);
            detailContentBackButton.gameObject.SetActive(false);
        }

#region Zzim Functions
        /// <summary>
        /// 서버에 찜 등록
        /// 버튼의 이미지도 바뀜
        /// </summary>
        /// <param name="_account">등록할 회원 번호</param>
        /// <param name="_marketId">등록할 가게 번호</param>
        /// <param name="_isFavorite"></param>
        /// <param name="button">이미지가 바뀔 버튼</param>
        /// <returns></returns>
        public async Task PostZzim(string _account, string _marketId, bool _isFavorite, Button button){
            string isFavorite = _isFavorite ? "True" : "False";
            var values = new Dictionary<string, string>{
                {"account", _account},
                {"market_id", _marketId},
                {"is_favorite", isFavorite}
            };

            string url = appSetting.baseURL + "create_favorite/";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            button.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = fullHeart;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(async () => await DeleteZzim(_account, _marketId, button));
        }
        /// <summary>
        /// 서버에서 찜 삭제
        /// </summary>
        /// <param name="_account"></param>
        /// <param name="_marketId"></param>
        /// <param name="_isFavorite"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        public async Task DeleteZzim(string _account, string _marketId, Button button){
            var values = new Dictionary<string, string>{
                {"account", _account},
                {"market_id", _marketId}
            };

            string url = appSetting.baseURL + "delete_favorite/";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            button.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = blankHeart;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(async () => await PostZzim(_account, _marketId, true, button));
        }

        /// <summary>
        /// UserProfile에 등록된 유저 번호를 이용하여
        /// 찜목록을 서버에서 받아오고 보여줌
        /// </summary>
        /// <returns></returns>
        public async void SearchZzimListAsync(){
            var zzimarr = await RequestZzimArray(UserProfile.social_login_id);
            //var zzimarr = await RequestZzimArray("2774886049");
            var stores = SearchByMarketId(zzimarr);
            await DisplayResult(stores);
        }

        /// <summary>
        /// 서버에 회원의 찜 목록을 요청
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task<int[]> RequestZzimArray(string account){
            string url =  baseURL + "get_favorite_market_ids/" + "?social_login_id=" + account;

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            var temp = JsonConvert.DeserializeObject<ZzimList>(result);
            int[] zzimarr = temp.market_ids;

            // 중복 제거
            int[] distArr = zzimarr.Distinct().ToArray();

            return distArr;
        }
#endregion

        private void ClearResultList()
        {
            foreach (GameObject item in resultPrefs)
            {
                Destroy(item);
            }
        }
    }
}
