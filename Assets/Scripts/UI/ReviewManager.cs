using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using ToonJido.Common;
using ToonJido.Control;
using ToonJido.Data.Model;
using ToonJido.Search;
using UnityEngine;
using UnityEngine.UI;

public class ReviewManager : MonoBehaviour
{
    public CanvasListItem reviewCanvas;
    public Button reviewCanvasCloseButton;
    public TextMeshProUGUI storeName;
    public GameObject reviewScrollView;
    public GameObject reviewContentParent;
    public GameObject noReviewText;

    public GameObject reviewPref;

    // common manager
    private HttpClient client;
    private SearchManager searchManager;
    void Start(){
        client = HttpClientProvider.GetHttpClient();
        searchManager = SearchManager.GetInstance();

        reviewCanvasCloseButton.onClick.AddListener(CloseReviewCanvas);
    }

    public async Task<review[]> GetReviewData(string _storeName){
        var result = await searchManager.SearchStore(_storeName);
        var reviews = result.cultures[0].reviews;

        print($"get reviews data number of reviews: {reviews.Length}");
        return reviews;
    }

    public void OpenReviewCanvas(){
        reviewCanvas.SetActive(true);
    }

    public void CloseReviewCanvas(){
        reviewCanvas.SetActive(false);
        
        // 리뷰들 제거
        if(0 < reviewContentParent.transform.childCount){
            for(int i = 0; i < reviewContentParent.transform.childCount; i++){
                print($"destory review object name: {reviewContentParent.transform.GetChild(i).name}");
                Destroy(reviewContentParent.transform.GetChild(i).gameObject);
            }
        }
    }

    public void DisplayReview(review[] reviews, string storeName){
        print($"display reviews, number of review: {reviews.Length}");

        this.storeName.text = storeName;

        if(0 < reviews.Length){
            reviewScrollView.SetActive(true);
            noReviewText.SetActive(false);
        }
        else{
            reviewScrollView.SetActive(false);
            noReviewText.SetActive(true);
            return;
        }

        foreach(var item in reviews){
            var reviewClone = Instantiate(reviewPref, reviewContentParent.transform);
            reviewClone.transform.Find("Nickname").GetComponent<TextMeshProUGUI>().text = $"사용자@{item.account}";
            reviewClone.transform.Find("Article").GetComponent<TextMeshProUGUI>().text = item.content;
        }
    }
}
