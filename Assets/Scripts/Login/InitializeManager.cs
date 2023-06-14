using UnityEngine;
using System.IO;
using System.Net.Http;
using ToonJido.Data.Saver;
using System.Threading.Tasks;
using static appSetting;
using ToonJido.Common;
using ToonJido.Data.Model;

namespace ToonJido.Login
{
    public class InitializeManager : MonoBehaviour
    {
        private SceneLoaderSingleton sceneLoader;
        private HttpClient client = HttpClientProvider.GetHttpClient();
        public string token;
        public GameObject warningCanvas;

        void Start()
        {
            sceneLoader = SceneLoaderSingleton.instance;

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
            await DownloadMarketDB();
            if(CheckTokenFile()){
                UserProfile.token = GetToken();
                UserProfile.social_login_id = await GetUserSocialIdAsync();
                sceneLoader.LoadSceneAsync("03 TestScene");
            }
            else{
                token = "no token";
                sceneLoader.LoadSceneAsync("02 FirstLoginScene");
            }
                
        }

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

        public async Task DownloadMarketDB()
        {
            var searchURL = baseURL + "get_Market_DB_List";
            var response = await client.GetAsync(searchURL);
            try{
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException){
                warningCanvas.SetActive(true);
                return;
            }
            var httpResult = await response.Content.ReadAsStringAsync();
            using(StreamWriter outputFile = new(dataPath)){
                await outputFile.WriteAsync(httpResult);
            }
        }

        public void OpenKakaoLogin(){
            Application.OpenURL(appSetting.baseURL + "kakaoGetLogin");
        }
    }
}

