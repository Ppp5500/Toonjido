using UnityEngine;
using System.IO;
using UnityEngine.UI;
using ToonJido.Data.Saver;
using ToonJido.Data.Model;

namespace ToonJido.Login
{
    public class LoginManager : MonoBehaviour
    {
        private SceneLoaderSingleton sceneLoader;

        void Start()
        {
            sceneLoader = SceneLoaderSingleton.instance;

            AppInitialization();
        }

        private void AppInitialization(){
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

        public void OpenKakaoLogin(){
            Application.OpenURL(appSetting.baseURL + "kakaoGetLogin");
        }
    }
}

