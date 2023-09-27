using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using static appSetting;

using TMPro;

using ToonJido.Common;
using ToonJido.Control;
using ToonJido.Data.Model;
using ToonJido.Login;
using ToonJido.UI;

using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

namespace ToonJido.Search
{
    public class SearchManager : MonoBehaviour
    {
        // Data from Init phase
        public SearchedStore storeData = new();
        [HideInInspector]public bool isDone = false;

        [Header("모달창 UI")]
        [SerializeField] private UIDrag drag;

        [Space(10)]
        [Header("검색 창 UI 요소")]
        [SerializeField] private Button searchButton;
        [SerializeField] private TMP_InputField inputField;

        [Header("검색 결과창 UI 요소")]
        [SerializeField] private Button findRoadButton;
        [SerializeField] private GameObject searchList;
        [SerializeField] private GameObject resultScroll;
        [SerializeField] private GameObject loadingText;
        [SerializeField] private GameObject resultParent;
        [SerializeField] private GameObject resultPref;
        [SerializeField] private GameObject noResultText;

        [Header("포커싱 기능을 위한 캠 타겟")]
        [SerializeField] private GameObject camTarget;

        [Header("길찾기 관련 컴포넌트들")]
        public NaverDirectionManager naverDirectionManager;
        public PathDrawer pathDrawer;
        public PlayerArrivalChecker playerArrivalChecker;
        public Transform player;


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
        [SerializeField] private TMP_InputField reviewInputField;
        [SerializeField] private TextMeshProUGUI reviewPlaceHolder;
        private const string noReviewMent = "리뷰를 남겨주세요.";
        private const string noLoginMent = "로그인하고 리뷰를 남겨주세요.";
        [SerializeField] private Button reviewCanvasOpenButton;
        [SerializeField] private UnityEngine.UI.Image[] stars = new UnityEngine.UI.Image[5];
        [SerializeField] private Button postReviewButton;
        [SerializeField] private Sprite blankStar;
        [SerializeField] private Sprite fullStar;
        [SerializeField] private Sprite blankHeart;
        [SerializeField] private Sprite fullHeart;
        private int selectedStarRank = 0;
#endregion DetailCanvasProperties

        [Space(50)]
        [Header("테스트 오브젝트들")]
        // test
        public GameObject iconObject;
        public GameObject signCanvas;

        // common managers
        private NoticeManager noticeManager;
        private ReviewManager reviewManager;
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
                    isDone = true;
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
            noticeManager = NoticeManager.GetInstance();
            reviewManager = GameObject.Find("ReviewManager").GetComponent<ReviewManager>();
        }

        public static SearchManager GetInstance(){
            return instance;
        }

#if DEVELOPMENT
        // private void OnGUI() {
        //     if(GUI.Button(new Rect(300, 500, 150, 150), "map!")){
        //         Application.OpenURL("https://map.naver.com/v5/directions/14137544.395059217,4476234.392605368/14153964.843715463,4412293.111933241/-/transit?c=11.23,0,0,0,dh");
        //     }

        //     if(GUI.Button(new Rect(300, 700, 150, 150), "map!2")){
        //         Application.OpenURL("nmap://route/public?slat=35.09789741939864&slng=129.03468293094306&sname=%EB%82%A8%ED%8F%AC%EB%8F%99&dlat=35.17982543369992&dlng=129.07499499992576&dname=%EB%B6%80%EC%82%B0%EC%8B%9C%EC%B2%AD");
        //     }
        // }
#endif

        public async void ClickSearchButton()
        {
            BackToSearchResult();
            if (!string.IsNullOrEmpty(inputField.text))
            {
                noResultText.SetActive(false);
                var searchResult = await SearchStore(inputField.text);

                if (searchResult.cultures.Length == 0)
                {
                    ClearResultList();
                    drag.Up();
                    noResultText.SetActive(true);
                }
                else
                {
                    await DisplayResult(searchResult);
                }
            }
            else
                return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async void SearchByNameAndDisplay(string name){
            var result = await SearchStore(name);
            await DisplayResult(result);
        }

        /// <summary>
        /// 서버에 검색어로 검색 요청
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public async Task<SearchedStore> SearchStore(string keyword)
        {
            // 서버 통신
            var searchURL = baseURL + "search/?query=" + keyword;
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();
            var searchResult = await response.Content.ReadAsStringAsync();
            
            // 내보낼 객체 생성
            SearchedStore result = new();
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            result = JsonConvert.DeserializeObject<SearchedStore>( searchResult, setting );

            return result;
        }

        /// <summary>
        /// 서버에 사용자의 리뷰와 함께 자세한 가게 정보 요청
        /// </summary>
        /// <returns></returns>
        public async Task<DetailInfo> GetDetailReview(string account, string market_id){
            string url = appSetting.baseURL + "get_favorite_review/" + "?social_login_id=" + account + "&market_id=" + market_id;
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();
            var searchResult = await response.Content.ReadAsStringAsync();
                        
            // 내보낼 객체 생성
            DetailInfo result = new();
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            result = JsonConvert.DeserializeObject<DetailInfo>( searchResult, setting );

            return result;
        }

        /// <summary>
        /// 초기화 단계에서 저장한 데이터에서 find_number가 일치하는 가게 검색
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public SearchedStore SearchByAddress(string address)
        {
            SearchedStore searchedStore = new();
            searchedStore.cultures = storeData.cultures
                .Where(x => x.find_number == address)
                .ToArray();
            return searchedStore;
        }

        /// <summary>
        /// section 내에서 category가 일치하는 가게들 검색 하여 Display
        /// </summary>
        /// <param name="category"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public async Task SearchByCategoryInSectionAsync(int category, int section){
            SearchedStore searchedStore = new();
            string sumSection = "Section" + section;
            searchedStore.cultures = storeData.cultures
                .Where(x => x.section == sumSection)
                .Where(x => x.category == category)
                .ToArray();
            await DisplayResult(searchedStore, "Half");
        }

        public async void SearchKeyStoresAsync(){
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

            result.cultures = storeData.cultures.Where(x => keyStoresName.Contains(x.market_name)).ToArray();
            await DisplayResult(result, popupWay: "Up");
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

        /// <summary>
        /// Oepn modal and instance result prefs
        /// </summary>
        /// <param name="input"></param>
        /// <param name="popupWay">"Up"=full screen(default), "Half"=half screen, "Down"=Down, "Nothing"=Do nothing</param>
        /// <returns></returns>
        public async Task DisplayResult(SearchedStore input, string popupWay = "Up")
        {
            noResultText.SetActive(false);
            ClearResultList();
            if(detailContentScroll.activeSelf) BackToSearchResult();
            switch(popupWay){
                case "Up":
                    drag.Up();
                    break;
                case "Half":
                    drag.Half();
                    break;
                case "Down":
                    drag.Down();
                    break;
                case "Nothing":
                    break;
                default:
                    break;
            }
            if(input.cultures.Length < 1) 
            {
                print("no result!");
                noResultText.SetActive(true);
            }

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
                    if(input.cultures[i].open_check != "O")
                    {
                        mypref.transform
                            .Find("Bottom")
                            .gameObject
                            .transform
                            .Find("Is Running")
                            .gameObject
                            .SetActive(false);
                    }
                    var tel = "-";
                    if(12 < input.cultures[i].phone.Length)
                    {
                        tel = Regex.Replace(input.cultures[i].phone, @"\D", "");
                        tel = tel.Insert(3, "-").Insert(7, "-");
                    }
                    mypref.transform
                        .Find("Bottom")
                        .gameObject
                        .transform
                        .Find("Tel Info")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(tel);
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

        public async void PathFindAsync(string address, string _marketName)
        {
#if DEVELOPMENT
            var tempMarket = await SearchStore(_marketName);
            var tempName = tempMarket.cultures[0].market_name;
            var lat = tempMarket.cultures[0].latitude;
            var lon = tempMarket.cultures[0].longitude;

            noticeManager
                .SetCancelButton(async ()
                    => {
                        Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(address);
                        // NavPlayerControl.navPlayerControl.SetDestination(lot, _marketName);
                        // noticeManager.CloseCanvas();
                        // drag.Half();
                        // FocusToHalf(lot);
                        var arr = await naverDirectionManager.GetPathFromNaver(player.position, lot);
                        pathDrawer.DrawLine(arr, tempName);
                        var index = arr.Length -1;
                        var destination = GPSEncoder.GPSToUCS((float)arr[index][1], (float)arr[index][0]);
                        playerArrivalChecker.SetArrivalChecker(destination);

                        // 출발지 좌표 계산
                        var startPoint = GPSEncoder.GPSToUCS((float)arr[0][1], (float)arr[0][0]);

                        noticeManager.CloseCanvas();
                        FocusToHalf(startPoint);
                    })
                .SetConfirmButton(()
                    => {
                        address = "https://map.kakao.com/link/to/" + _marketName + "," + lat + "," + lon;
                        Application.OpenURL(address);
                        print(address);
                    })
                .ShowNotice("현재 개발 모드입니다! 강제 길찾기는 취소버튼을, 외부 길찾기 링크는 확인버튼을 눌러주세요.");

#else
            var tempMarket = await SearchStore(_marketName);
            var tempName = tempMarket.cultures[0].market_name;
            if(CurrentControl.gpsStatus == GPSStatus.avaliable){
                Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(address);

                // 네이버 길찾기 정보 획득
                var arr = await naverDirectionManager.GetPathFromNaver(player.position, lot);

                // 획득한 정보로 경로 그리기
                pathDrawer.DrawLine(arr, tempName);

                // 목적지 도착 검사 설정
                var index = arr.Length -1;
                var destination = GPSEncoder.GPSToUCS((float)arr[index][1], (float)arr[index][0]);
                playerArrivalChecker.SetArrivalChecker(destination);

                // 출발지 좌표 계산
                var startPoint = GPSEncoder.GPSToUCS((float)arr[0][1], (float)arr[0][0]);

                // UI작동
                noticeManager.CloseCanvas();
                drag.Half();
                FocusToHalf(startPoint);
            }
            else{
                var lat = tempMarket.cultures[0].latitude;
                var lon = tempMarket.cultures[0].longitude;
                noticeManager.SetCancelButtonDefault()
                    .SetConfirmButton(()
                        => {
                            address = "https://map.kakao.com/link/to/" + _marketName + "," + lat + "," + lon;
                            Application.OpenURL(address);
                            print(address);
                        })
                    .ShowNotice("명지역길 외부에서는 길찾기를 사용할 수 없습니다. 외부 길찾기 기능을 여시겠습니까?");
            }
#endif
        }

        public void FocusToHalf(Vector3 target){
            var cam = Camera.main;
            var camWidth = cam.pixelWidth;
            var camHeight = cam.pixelHeight;

            Vector3 viewPos = cam.ScreenToWorldPoint(new Vector3(camWidth/2, camHeight/3, 500));
            Vector3 centerPos = cam.ScreenToWorldPoint(new Vector3(camWidth/2, camHeight/2, 500));
            var temp = viewPos - centerPos;
            Vector3 lot = new Vector3(target.x, 0, target.z + temp.z);

            StartCoroutine(FocusCoroutine(lot));
        }

        IEnumerator FocusCoroutine(Vector3 target){
            float t = 0.0f;
            Vector3 startPosition = camTarget.transform.position;

            while(t < 1){
                t += Time.deltaTime/0.5f;
                camTarget.transform.position = Vector3.Lerp(startPosition, target, t);
                yield return null;
            }
        }

        private void ModalChoice()
        {
            if(CurrentControl.state == State.Eyelevel)
            {
                drag.Down();
            }
            else if(CurrentControl.state == State.Overlook)
            {
                drag.Half();
            }
        }

        public async void OpenDetailCanvas(string name)
        {
            pathFindButton.onClick.RemoveAllListeners();

            // show detailCanvas
            detailContentScroll.SetActive(true);
            loadingPanel.SetActive(true);
            resultScroll.SetActive(false);

            // Get Store data
            var result = await SearchStore(name);
            if(result.cultures.Length < 1){
                noticeManager.ShowNoticeDefaultStyle("해당 매장은 업데이트 예정입니다.");
                return;
            }
            Culture currentStore = result.cultures[0];
            if(currentStore.cawarock == string.Empty){
                noticeManager.ShowNoticeDefaultStyle("해당 매장은 업데이트 예정입니다.");
                return;
            }

            drag.Up();

            var market_id = currentStore.id;

            // 이미지 다운로드
            string imgURL = currentStore.images[0].image;
                    var data = await client.GetByteArrayAsync(baseURL + imgURL.Substring(1));
                    Texture2D tex = new(690, 500);
                    tex.LoadImage(data, false);
                    Rect rect = new Rect(0, 0, tex.width, tex.height);
                    Sprite sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));

            // detailCanvas에 적용
            var find_number = currentStore.find_number;
            Thumbnail.sprite = sp;
            storeName.text = currentStore.market_name;
            starText.text = currentStore.average_grade.ToString();
            explainText.text = currentStore.explain;
            clockText.text = currentStore.open_hours;
            addressText.text = currentStore.address;
            contactText.text = "-";
            if (12 < currentStore.phone.Length)
            {
                contactText.text = Regex.Replace(currentStore.phone, @"\D", "");
                contactText.text = contactText.text.Insert(3, "-").Insert(7, "-");
            }
            if(currentStore.latitude != null){
                print($"lat: {currentStore.latitude}");

                GameObject obj = Instantiate(iconObject);
                obj.transform.parent = signCanvas.transform;
                obj.transform.position = GPSEncoder.GPSToUCS((float)currentStore.latitude, (float)currentStore.longitude);
                obj.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
            }
            

            // 길찾기 버튼
            pathFindButton.onClick.AddListener(() => PathFindAsync(find_number, currentStore.market_name));
            pathFindButton.onClick.AddListener(() => ModalChoice());

            reviewCanvasOpenButton.onClick.AddListener(() => {
                reviewManager.OpenReviewCanvas();
                reviewManager.DisplayReview(currentStore.reviews, currentStore.market_name);
                reviewCanvasOpenButton.onClick.RemoveAllListeners();
            });

            print($"current id: {UserProfile.social_login_id}");
            if(UserProfile.social_login_id != string.Empty)
            {
                // 찜 목록을 다운 받아서 이미 찜한 상점인지 검사
                var zzimArr = await RequestZzimArray(UserProfile.social_login_id);
                bool isZzim = zzimArr.Contains(market_id);

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

                // Get Review data
                var reviewResult = await GetDetailReview(UserProfile.social_login_id, market_id.ToString());
                
                // 점수 계산
                // int rank = Mathf.RoundToInt(currentStore.average_grade);
                int rank = reviewResult.review_grades.Length > 0 ? reviewResult.review_grades[0] : 1;
                
                // 자신이 쓴 리뷰가 없으면
                if(reviewResult.review_contents.Length < 1){
                    print("Not exist my review");
                    reviewPlaceHolder.text = noReviewMent;
                    // 별들을 비어 있는 별로
                    foreach(var item in stars){
                        item.sprite = blankStar;
                        item.GetComponent<Button>().interactable = true;
                    }

                    reviewInputField.interactable = true;
                    reviewInputField.text = string.Empty;
                    postReviewButton.onClick.AddListener(async () => await PostReviewAsync(UserProfile.social_login_id, market_id.ToString(), reviewInputField.text, selectedStarRank.ToString()));
                    postReviewButton.interactable = false;
                }
                // 자신이 쓴 리뷰가 있으면
                else{
                    print("Exist my review");
                    for(int j = 0; j < rank; j++){
                        stars[j].sprite = fullStar;
                    }
                    for(int k = rank; k < 5; k++){
                        stars[k].sprite = blankStar;
                    }

                    foreach(var item in stars){
                        item.GetComponent<Button>().interactable = false;
                    }

                    reviewInputField.text = reviewResult.review_contents[0];
                    reviewInputField.interactable = false;
                    postReviewButton.interactable = false;
                }
            }
            else
            {
                zzimButton.interactable = false;
                foreach(var item in stars){
                    item.GetComponent<Button>().interactable = false;
                }
                postReviewButton.interactable = false;
                reviewInputField.interactable = false;
                reviewPlaceHolder.text = noLoginMent;
            }

            // 상세 화면 표시
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

        public void CheckReadyPostReview(){
            if(reviewInputField.text is not null && selectedStarRank is not 0){
                postReviewButton.interactable = true;
            }
            else{
                postReviewButton.interactable = false;
            }
        }

        public void DrawingStars(int input){
            selectedStarRank = input;
            for(int j = 0; j < selectedStarRank; j++){
                stars[j].sprite = fullStar;
            }
            for(int k = selectedStarRank; k < 5; k++){
                stars[k].sprite = blankStar;
            }
        }

#region Zzim Methods
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
                //{"account", "2774886049"},
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
        /// Request Zzim Array를 호출하여 서버에서 찜목록을 받아오고 보여줌
        /// </summary>
        /// <returns></returns>
        public async void SearchZzimListAsync(){
            var zzimarr = await RequestZzimArray(UserProfile.social_login_id);
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

#region Review Methods
        public async Task PostReviewAsync(string account, string market_id, string article, string grade){
            var values = new Dictionary<string, string>{
                {"account", account},
                {"market_id", market_id},
                {"content", article},
                {"grade", grade}
            };

            string url = appSetting.baseURL + "create_review/";
            var data = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(url, data);

            if(response.EnsureSuccessStatusCode().IsSuccessStatusCode){
                noticeManager.ShowNoticeDefaultStyle("리뷰 등록이 완료되었습니다.");
                postReviewButton.interactable = false;
                reviewInputField.interactable = false;

                foreach(var item in stars){
                    item.GetComponent<Button>().interactable = false;
                }
            }
            var result = await response.Content.ReadAsStringAsync();
        }

        public void DeleteReview(){

        }

        public void SearchReviewList(){
            
        }

        public void RequestReviewArrayToServer(){
            
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