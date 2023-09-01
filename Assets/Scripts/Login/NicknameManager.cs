using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using static appSetting;

using TMPro;

using ToonJido.Common;
using ToonJido.Login;

using UnityEngine;
using UnityEngine.UI;
using ToonJido.Control;
using ToonJido.UI;

public class NicknameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasListItem nicknameSettingCanvas;
    public TextMeshProUGUI nicknameText;
    public Image profilePic;
    public Image profilePicOnCanvas;
    public TMP_InputField nicknameInputfield;
    public Button closeButton;

    [Header("Profile Picture Sprites")]
    public List<Sprite> profileSprites = new();

    [Header("Profile Select Buttons")]
    public List<Button> profileSelectButtons = new();

    [Header("Profile Picture Frame Sprites")]
    public Sprite selected;
    public Sprite notSelected;

    private int? currPicNum;
    // common
    HttpClient client = HttpClientProvider.GetHttpClient();

    async Task Start()
    {
        print("profile start!");
        if(UserProfile.social_login_id != string.Empty){
            print("profile init!");

            await InitProfileAsync();
            for(int i = 0; i < profileSelectButtons.Count; i++){
                int tempNum = i;
                profileSelectButtons[tempNum].onClick.AddListener(() => SelectPic(tempNum));
            }

            closeButton.onClick.AddListener(() => CloseCanvas());
            nicknameInputfield.onValueChanged.AddListener((_input) => EditNickname(_input));
        }
        else
        {
            nicknameText.text = UserProfile.social_login_id;
        }
    }

#if DEVELOPMENT
    private async Task OnGUI() {
        if(GUI.Button(new Rect(450, 450, 150, 150), "save nickname")){
            await SaveProfileDataOnDevice("{\"account_id\": \"2774886049\", \"nickname\": \"\ub2c9\ub124\uc784\ub9cc \ud14c\uc2a4\ud2b8\", \"profile_img\": \"2\"}");
        }

        if(GUI.Button(new Rect(450, 650, 150, 150), "load nickname")){
            await LoadProfileDataOnDevice();
        }
    }
#endif

    async Task InitProfileAsync(){
        string loadedData = string.Empty;
        print("file exist?: " + File.Exists(nicknameDataPath));
        loadedData = File.Exists(nicknameDataPath) ? await LoadProfileDataOnDevice() : await GetDataSavedOnServer();
        print("server data" + loadedData);
        var loadedJObject = JObject.Parse(loadedData);

        nicknameInputfield.text = loadedJObject["nickname"].ToString();
        DisplayNickname(loadedJObject);
        DisplayProfilePic(loadedJObject);
    }

    public async void SelectPic(int _num){
        DisplayProfilePic(_num);
        await SetProfilPicOnServer(_num);
    }

    public async void EditNickname(string _nickname){
        print("edit!");
        DisplayNickname(_nickname);
        await SetNicknameOnServer(_nickname);
    }

    public void OpenCanvas(){
        if(UserProfile.social_login_id != string.Empty){
            nicknameSettingCanvas.SetActive(true);
        }
        else{
            NoticeManager.GetInstance().ShowNoticeDefaultStyle("비로그인 시에는 프로필을 설정할 수 없습니다.");
        }
    }

    public void CloseCanvas(){
        nicknameSettingCanvas.SetActive(false);
    }

#region UI Display Methods
    void DisplayNickname(JObject _jo){
        nicknameText.text = _jo["nickname"].ToString();
    }
    void DisplayNickname(string _nickname){
        nicknameText.text = _nickname;
    }
    void DisplayProfilePic(JObject _jo){
        profilePic.sprite = profileSprites[(int)_jo["profile_img"]];
        profilePicOnCanvas.sprite = profileSprites[(int)_jo["profile_img"]];

        var nextPicFrame = profileSelectButtons[(int)_jo["profile_img"]].transform.Find("Circle Frame").GetComponent<Image>();
        nextPicFrame.sprite = selected;

        if(currPicNum != null){
            var currPicFrame = profileSelectButtons[(int)currPicNum].transform.Find("Circle Frame").GetComponent<Image>();
            currPicFrame.sprite = notSelected;
        }

        currPicNum = (int)_jo["profile_img"];
    }
    void DisplayProfilePic(int _num){
        profilePic.sprite = profileSprites[_num];
        profilePicOnCanvas.sprite = profileSprites[_num];

        var nextPicFrame = profileSelectButtons[_num].transform.Find("Circle Frame").GetComponent<Image>();
        nextPicFrame.sprite = selected;

        if(currPicNum != null){
            var currPicFrame = profileSelectButtons[(int)currPicNum].transform.Find("Circle Frame").GetComponent<Image>();
            currPicFrame.sprite = notSelected;
        }

        currPicNum = _num;
    }
#endregion

#region Server Communication && Device Save Methods
    async Task<string> GetDataSavedOnServer(){
        var url = baseURL + "get_user_profile/";
        var response = await client.GetAsync(url + "?social_login_id=" + UserProfile.social_login_id);

        if (!response.IsSuccessStatusCode)
        {
            await InitServerData();
            var result = await LoadProfileDataOnDevice();
            return result;
        }
        else
        {
            var result = await response.Content.ReadAsStringAsync();
            return result;

        }
    }

    async Task InitServerData()
    {
        await SetDataOnServer("흥룡이 프렌드", 0);
    }

    async Task SetDataOnServer(string _nickName, int _num){
        var url = baseURL + "save_user_info/";

        var values = new Dictionary<string, string>{
            {"account_id", UserProfile.social_login_id},
            {"nickname", _nickName},
            {"profile_img", _num.ToString()}
        };
        
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        if(response.IsSuccessStatusCode){
            await SaveProfileDataOnDevice(UserProfile.social_login_id, _nickName, _num.ToString());
        }
        else{
            print("something worng!");
        }
#if DEVELOPMENT
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
#endif
    }

    async Task SetNicknameOnServer(string _nickName){
        var url = baseURL + "save_user_info/";
        string profileImgNum = "1";

        // 저장된 파일에서 프로필 이미지 번호 확인
        if(File.Exists(nicknameDataPath)){
            var loadedData = await LoadProfileDataOnDevice();
            var loadedJObject = JObject.Parse(loadedData);
            profileImgNum = loadedJObject["profile_img"].ToString();
        }

        var values = new Dictionary<string, string>{
            {"account_id", UserProfile.social_login_id},
            {"nickname", _nickName},
            {"profile_img", profileImgNum}
        };

        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        if(response.IsSuccessStatusCode){
            await SaveProfileDataOnDevice(UserProfile.social_login_id, _nickName, profileImgNum);
        }
        else{
            print("something worng!");
        }
#if DEVELOPMENT
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
#endif
    }

    async Task SetProfilPicOnServer(int _num){
        var url = baseURL + "save_user_info/";
        string nickname = "흥룡이 프렌즈";

        // 저장된 파일에서 닉네임 확인
        if(File.Exists(nicknameDataPath)){
            var loadedData = await LoadProfileDataOnDevice();
            var loadedJObject = JObject.Parse(loadedData);
            nickname = loadedJObject["nickname"].ToString();
        }
        
        var values = new Dictionary<string, string>{
            {"account_id", UserProfile.social_login_id},
            {"nickname", nickname},
            {"profile_img", _num.ToString()}
        };
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        if(response.IsSuccessStatusCode){
            await SaveProfileDataOnDevice(UserProfile.social_login_id, nickname, _num.ToString());
        }
        else{
            print("something worng!");
        }
#if DEVELOPMENT
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
#endif
    }

    async Task SaveProfileDataOnDevice(string _values){
        await File.WriteAllTextAsync(nicknameDataPath, _values);
    }

    async Task SaveProfileDataOnDevice(string _accountID, string _nickname, string _picNum){
        JObject asdf = new(
            new JProperty("account_id", _accountID),
            new JProperty("nickname", _nickname),
            new JProperty("profile_img", _picNum)
        );

        await File.WriteAllTextAsync(nicknameDataPath, asdf.ToString());
    }

    async Task<string> LoadProfileDataOnDevice(){
        var loadedString = await File.ReadAllTextAsync(nicknameDataPath);
        return loadedString;
    }
#endregion
}