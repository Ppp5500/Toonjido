using UnityEngine;
using System.IO;
using System.Net.Http;
using ToonJido.Data.Saver;
using System.Threading.Tasks;
using static appSetting;
using ToonJido.Common;
using ToonJido.UI;

namespace ToonJido.Login
{
    public class InitializeManager : MonoBehaviour
    {
        // common managers
        private SceneLoaderSingleton sceneLoader;
        private NoticeManager noticeManager;
        private HttpClient client;

        void Start()
        {
            sceneLoader = SceneLoaderSingleton.instance;
            noticeManager = NoticeManager.GetInstance();
            client = HttpClientProvider.GetHttpClient();

            AppInitialization();
        }

#if DEVELOPMENT
        private void OnGUI()
        {
            // Compute a fontSize based on the size of the screen width.
            // GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            // GUI.Label(new Rect(20, 200, 500, 500),
            //      $"token:{token}");
        }
#endif
        private async void AppInitialization(){
            bool netConnection = await DownloadMarketDB();

            if(netConnection)
            {
                if(PlayerPrefs.HasKey(AppleUserIdKey))
                {
                    UserProfile.social_login_id = PlayerPrefs.GetString(AppleUserIdKey);
                    UserProfile.curr_id_type = IdType.apple;
                    sceneLoader.LoadSceneAsync("03 TestScene");
                }
                else if(PlayerPrefs.HasKey(KakaoUserIdKey))
                {
                    UserProfile.social_login_id = PlayerPrefs.GetString(KakaoUserIdKey);
                    UserProfile.curr_id_type = IdType.kakao;
                    sceneLoader.LoadSceneAsync("03 TestScene");
                }
                else{
                    sceneLoader.LoadSceneAsync("02 FirstLoginScene");
                }
            }
        }

        public async Task<bool> DownloadMarketDB()
        {
            var searchURL = baseURL + "get_Market_DB_List";
            
            try{
                var response = await client.GetAsync(searchURL);
                var httpResult = await response.Content.ReadAsStringAsync();
                using(StreamWriter outputFile = new(dataPath))
                {
                    await outputFile.WriteAsync(httpResult);
                }
            }
            catch(HttpRequestException){
                noticeManager.SetConfirmButton(() => Application.Quit());
                noticeManager.DisableCancelButton();
                noticeManager.ShowNotice("서버와의 연결에 실패했습니다. 어플리케이션을 종료합니다.");
                return false;
            }

            return true;
        }

        // not using
        private bool CheckTokenFile(){
            return File.Exists(appSetting.tokenPath);
        }

        private string GetToken(){
            using(PlayerDataSaver saver = new()){
                return saver.LoadToken();
            }
        }

        private async Task<string> GetUserSocialIdAsync(){
            using(PlayerDataSaver saver = new()){
                return await saver.LoadUserSocialIdAsync();
            }
        }
    }
}

