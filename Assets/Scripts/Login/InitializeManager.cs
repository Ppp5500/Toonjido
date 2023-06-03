using UnityEngine;
using System.IO;
using System.Net.Http;
using ToonJido.Data.Saver;
using System.Threading.Tasks;
using static appSetting;
using ToonJido.Common;

namespace ToonJido.Login
{
    public class InitializeManager : MonoBehaviour
    {
        private SceneLoaderSingleton sceneLoader;
        private HttpClient client = HttpClientProvider.GetHttpClient();

        void Start()
        {
            sceneLoader = SceneLoaderSingleton.instance;

            AppInitialization();
        }

        private async void AppInitialization(){
            //DownloadMarketDB();
            await DownloadMarketDB();
            if(CheckTokenFile()){
                sceneLoader.LoadSceneAsync("03 TestScene");
            }
            else
                sceneLoader.LoadSceneAsync("02 FirstLoginScene");
        }

        private bool CheckTokenFile(){
            return File.Exists(appSetting.tokenPath);
        }

        private void GetToken(string Input){
            using(PlayerDataSaver saver = new()){
                Input = saver.LoadToken();
            }
        }

        public async Task DownloadMarketDB()
        {
            var searchURL = baseURL + "get_Market_DB_List";
            var response = await client.GetAsync(searchURL);
            response.EnsureSuccessStatusCode();
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

