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

public class NicknameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI nicknameText;
    public Image profilePic;

    [Header("Profile Picture Sprites")]
    public List<Sprite> profileSprites;

    private int currPicNum;
    // common
    HttpClient client = HttpClientProvider.GetHttpClient();

    async Task Start()
    {
        await InitProfileAsync();
    }

#if DEVELOPMENT
    private async Task OnGUI() {
        if(GUI.Button(new Rect(450, 250, 150, 150), "post! nickname")){
            await SetNicknameOnServer("닉네임만 테스트");
        }

        if(GUI.Button(new Rect(450, 450, 150, 150), "get nickname")){
            await GetDataSavedOnServer();
        }

        if(GUI.Button(new Rect(450, 650, 150, 150), "save nickname")){
            await SaveProfileDataOnDevice("{\"account_id\": \"2774886049\", \"nickname\": \"\ub2c9\ub124\uc784\ub9cc \ud14c\uc2a4\ud2b8\", \"profile_img\": \"2\"}");
        }

        if(GUI.Button(new Rect(450, 850, 150, 150), "load nickname")){
            await LoadProfileDataOnDevice();
        }
    }
#endif

    async Task InitProfileAsync(){
        string loadedData = string.Empty;
        loadedData = File.Exists(nicknameDataPath) ? await LoadProfileDataOnDevice() : await GetDataSavedOnServer();

        var loadedJObject = JObject.Parse(loadedData);
        DisplayNickname(loadedJObject);
        DisplayProfilePic(loadedJObject);
    }

    public async void EditNickname(string _nickname){
        DisplayNickname(_nickname);
        await SetNicknameOnServer(_nickname);
    }

    public async void SelectPic(int _num){
        DisplayProfilePic(_num);
        await SetProfilPicOnServer(_num);
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
    }
    void DisplayProfilePic(int _num){
        profilePic.sprite = profileSprites[_num];
    }
#endregion

#region Server Communication && Device Save Methods
    async Task<string> GetDataSavedOnServer(){
        var url = baseURL + "get_user_profile/";
        var response = await client.GetAsync(url + "?social_login_id=" + UserProfile.social_login_id);

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadAsStringAsync();

        return result;
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

        response.EnsureSuccessStatusCode();
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

        response.EnsureSuccessStatusCode();
#if DEVELOPMENT
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
#endif
    }

    async Task SetProfilPicOnServer(int _num){
        var url = baseURL + "save_user_info/";
        string nickname = "흥룡이 프렌즈";

        // 저장된 파일에서 프로필 이미지 번호 확인
        if(File.Exists(nicknameDataPath)){
            var loadedData = await LoadProfileDataOnDevice();
            var loadedJObject = JObject.Parse(loadedData);
            nickname = loadedJObject["nickname"].ToString();
        }

        // 저장된 파일에서 닉네임 확인
        var values = new Dictionary<string, string>{
            {"account_id", UserProfile.social_login_id},
            {"nickname", nickname},
            {"profile_img", _num.ToString()}
        };
        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        response.EnsureSuccessStatusCode();
#if DEVELOPMENT
        var result = await response.Content.ReadAsStringAsync();
        print(response.ToString());
        print(result);
#endif
    }

    async Task SaveProfileDataOnDevice(string _values){
        print($"save profile: {_values}");
        await File.WriteAllTextAsync(nicknameDataPath, _values);
    }

    async Task<string> LoadProfileDataOnDevice(){
        var loadedString = await File.ReadAllTextAsync(nicknameDataPath);
        print($"loadedString: {loadedString}");
        return loadedString;
    }
#endregion
}