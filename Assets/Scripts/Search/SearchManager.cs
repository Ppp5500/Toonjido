using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using TMPro;
using ToonJido.Control;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using ToonJido.Common;
using ToonJido.UI;
using static appSetting;

namespace ToonJido.Search
{
    public class SearchManager : MonoBehaviour
    {
        SearchedStore store = new();

        [SerializeField]
        private UIDrag drag;

        [SerializeField]
        private Button searchButton;

        [SerializeField]
        private TMP_InputField inputField;

        [SerializeField]
        private GameObject searchBG;

        [SerializeField]
        private GameObject searchList;

        [SerializeField]
        private GameObject loadingText;

        [SerializeField]
        private GameObject resultParent;

        [SerializeField]
        private GameObject resultPref;

        [SerializeField]
        private GameObject noResultText;

        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Sprite blankHeart;

        [SerializeField]
        private Sprite fullHeart;
        private HttpClient client = HttpClientProvider.GetHttpClient();

        private List<GameObject> resultPrefs = new List<GameObject>();
        public static SearchManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        void Start()
        {
            CurrentControl.overlookAction += SearchPanelSwitch;
            CurrentControl.eyelevelAction += SearchPanelSwitch;
            CurrentControl.searchResultAction += SearchPanelSwitch;
            CurrentControl.profileAction += SearchPanelSwitch;

            drag = searchList.GetComponent<UIDrag>();

            backButton.onClick.AddListener(() => CurrentControl.ChangeToLastState());

            searchButton.onClick.AddListener(() => ClickSearchButton());
        }

        public async void ClickSearchButton()
        {
            ClearResultList();

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
                    DisplayResult(searchResultArr);
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

        public async Task<SearchedStore> SearchStoreByAddress(string address)
        {
            var searchURL = baseURL + "get_Market_DB_List";
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();
            var httpResult = await response.Content.ReadAsStringAsync();

            SearchedStore searchedStore = new();

            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            searchedStore = JsonConvert.DeserializeObject<SearchedStore>(httpResult, setting);

            searchedStore.cultures = searchedStore.cultures
                .Where(x => x.lot_number == address)
                .ToArray();
            return searchedStore;
        }

        public async void DisplayResult(SearchedStore input)
        {
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
                    button.interactable = false;
                }
                else
                {
                    string imgURL = input.cultures[i].images[0].image;
                    var data = await client.GetByteArrayAsync(baseURL + imgURL.Substring(1));
                    Texture2D tex = new(120, 120);
                    tex.LoadImage(data, false);
                    Rect rect = new Rect(0, 0, tex.width, tex.height);
                    Sprite sp = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));

                    mypref.transform
                        .Find("Main Image")
                        .GetComponent<UnityEngine.UI.Image>()
                        .sprite = sp;
                    string storeName = input.cultures[i].market_name;
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
                        // 나중에 변경
                        .SetText("4");
                    mypref.transform
                        .Find("Time Info")
                        .GetComponent<TextMeshProUGUI>()
                        .SetText(input.cultures[i].open_hours);
                    // 일단 open_check로 넣어놨는데 유저가 찜을 했는지 검사하는 걸로 바꿔야 됨
                    mypref.transform
                        .Find("Heart Icon")
                        .GetComponent<UnityEngine.UI.Image>()
                        .sprite =
                        input.cultures[i].open_check == "O" ? blankHeart : fullHeart;
                    var lot = input.cultures[i].lot_number;
                    mypref.transform
                        .Find("PathFindButton")
                        .GetComponent<Button>()
                        .onClick.AddListener(() => PathFind(lot));
                    // scroll content의 가장 위에 삽입
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
        }

        private void OpenDetailCanvas(string name){

        }

        private void SearchPanelSwitch()
        {
            if (CurrentControl.state == CurrentControl.State.SearchResult)
            {
                searchBG.SetActive(true);
            }
            else
            {
                ClearResultList();
                searchBG.SetActive(false);
            }
        }

        private void ClearResultList()
        {
            foreach (GameObject item in resultPrefs)
            {
                Destroy(item);
            }
        }
    }
}
