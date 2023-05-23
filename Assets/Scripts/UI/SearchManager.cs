using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using ToonJido.Control;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using ToonJido.Data.Model;
using static appSetting;

namespace ToonJido.UI
{
    public class SearchManager : MonoBehaviour
    {
        SearchedStore store = new();

        [SerializeField] private Button searchButton;
        [SerializeField] private TMP_InputField inputField;

        [SerializeField] private GameObject searchBG;
        [SerializeField] private GameObject searchList;
        [SerializeField] private GameObject resultParent;
        [SerializeField] private GameObject resultPref;
        [SerializeField] private GameObject noResultText;
        [SerializeField] private Button backButton;

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

            backButton.onClick.AddListener(() => CurrentControl.ChangeToLastState());

            searchButton.onClick.AddListener(() => ClickSearchButton());
        }

        public async void ClickSearchButton()
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                CurrentControl.ChangeToSearchResult();
                noResultText.SetActive(false);
                var searchResult = await SearchStore(inputField.text);
                SearchedStore searchResultArr = new();
                searchResultArr = JsonConvert.DeserializeObject<SearchedStore>(searchResult);
                if (searchResultArr.cultures.Length == 0)
                {
                    ClearResultList();
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
            using (HttpClient client = new())
            {
                var searchURL = baseURL + "search/?query=" + keyword;
                var response = await client.GetAsync(searchURL);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        private void DisplayResult(SearchedStore input)
        {
            for (int i = 0; i < input.cultures.Length; i++)
            {
                GameObject mypref = Instantiate(resultPref, resultParent.transform);

                mypref.transform.Find("name").GetComponent<TextMeshProUGUI>()
                    .SetText(input.cultures[i].name);
                mypref.transform.Find("explanation").GetComponent<TextMeshProUGUI>()
                    .SetText(input.cultures[i].explanation);
                resultPrefs.Add(mypref);
            }
        }

        private void SearchPanelSwitch()
        {
            if (CurrentControl.state == CurrentControl.State.SearchResult)
            {
                searchBG.SetActive(true);
                searchList.SetActive(true);
            }
            else
            {
                ClearResultList();
                searchBG.SetActive(false);
                searchList.SetActive(false);
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