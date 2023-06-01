using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using TMPro;
using ToonJido.Control;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using static appSetting;
using System.Collections;

namespace ToonJido.UI
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
        private GameObject resultParent;

        [SerializeField]
        private GameObject resultPref;

        [SerializeField]
        private GameObject noResultText;

        [SerializeField]
        private Button backButton;

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
                print(searchResult);
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
            //var searchURL = baseURL + "search/?query=" + address;
            var searchURL = baseURL + "get_Market_DB";
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();
            var httpResult = await response.Content.ReadAsStringAsync();

            SearchedStore searchedStore = new();

            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                StringEscapeHandling =StringEscapeHandling.Default
            };
            print(httpResult);
            httpResult = httpResult.Substring(2, httpResult.Length - 4);
            print(httpResult);
            httpResult = "{\\\"cultures\\\" : " + httpResult + "}";
            print(httpResult);
            searchedStore = JsonConvert.DeserializeObject<SearchedStore>(httpResult, setting);

            searchedStore.cultures = searchedStore.cultures
                .Where(x => x.lot_number == address)
                .ToArray();
            return searchedStore;
        }

        public void DisplayResult(SearchedStore input)
        {
            for (int i = 0; i < input.cultures.Length; i++)
            {
                GameObject mypref = Instantiate(resultPref, resultParent.transform);

                mypref.transform
                    .Find("Store Name")
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(input.cultures[i].market_name);
                mypref.transform
                    .Find("explanation")
                    .GetComponent<TextMeshProUGUI>()
                    .SetText(input.cultures[i].explain);
                resultPrefs.Add(mypref);
            }

            drag.Up();
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
