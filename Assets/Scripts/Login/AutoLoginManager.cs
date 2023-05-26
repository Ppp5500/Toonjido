using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AutoLoginManager : MonoBehaviour
{
    private SceneLoaderSingleton sceneLoader;

    private void Awake() {
        sceneLoader = SceneLoaderSingleton.instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void AppInitialization(){
        print(sceneLoader.GetCurrentSceneName());
        if(CheckTokenFile()){
            sceneLoader.LoadScene("03 TestScene");
        }
        else
            sceneLoader.LoadScene("02 FirstLoginScene");
    }

    private bool CheckTokenFile(){
        return File.Exists(appSetting.tokenPath);
    }
}
