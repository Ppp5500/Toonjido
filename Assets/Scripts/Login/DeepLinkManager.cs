using System;
using UnityEngine;
using ToonJido.Data.Saver;

namespace ToonJido.Login
{
    public class DeepLinkManager : MonoBehaviour
    {
        public static DeepLinkManager Instance { get; private set; }
        [Header("This object is not destroy on load")]

        public string deeplinkURL;
        private string token;
        private string loadtoken;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Application.deepLinkActivated += onDeepLinkActivated;
                if (!String.IsNullOrEmpty(Application.absoluteURL))
                {
                    // Cold start and Application.absoluteURL not null so process Deep Link.
                    onDeepLinkActivated(Application.absoluteURL);
                }
                // Initialize DeepLink Manager global variable.
                else deeplinkURL = "[none]";
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

#if DEVELOPMENT
        private void OnGUI()
        {
            //GUI.backgroundColor = Color.yellow;
            //// Compute a fontSize based on the size of the screen width.
            //GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            //GUI.Box(new Rect(20, 50, 400, 400), "DebugCube");
            //GUI.Label(new Rect(20, 150, 700, 300), $"token: {token}");

        }
#endif

        private void onDeepLinkActivated(string url)
        {
            // Update DeepLink Manager global variable, so URL can be accessed from anywhere.
            deeplinkURL = url;

            // Decode the URL to determine action. 
            // In this example, the app expects a link formatted like this:
            // unitydl://mylink?scene1 
            token = url.Split("?"[0])[1];

            using (PlayerDataSaver saver = new())
            {
                saver.SaveToken(token);
            }
        }

        public void SaveToken(string token)
        {
            using (PlayerDataSaver saver = new())
            {
                saver.SaveToken(token);
            }
        }

        public string LoadToken()
        {
            using (PlayerDataSaver saver = new())
            {
                var result = saver.LoadToken();
                loadtoken = result;
                return result;
            }
        }
    }
}

