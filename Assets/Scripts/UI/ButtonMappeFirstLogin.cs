using UnityEngine;
using UnityEngine.UI;
using ToonJido.Login;

namespace ToonJido.UI{
    public class ButtonMappeFirstLogin : MonoBehaviour
    {
        private SceneLoaderSingleton sl;
        private InitializeManager lm;
        private SampleWebView sw;
        private DeepLinkManager dm;
        private AppleManager am;

        [SerializeField] private Button tutorialCheckButton;
        [SerializeField] private Button appleLoginButton;
        [SerializeField] private Button kakaoLoginButton;
        [SerializeField] private Button skipLoginButton;
        [SerializeField] private GameObject signupCanvas;

        void Start()
        {
            sl = GameObject.Find("SceneLoader").GetComponent<SceneLoaderSingleton>();
            tutorialCheckButton.onClick.AddListener(() => sl.LoadSceneAsync("03 TestScene"));
            
#if UNITY_IOS
            sw = GameObject.Find("WebviewManager").GetComponent<SampleWebView>();
            kakaoLoginButton.onClick.AddListener(() => sw.OpenWebViewerCaller());
            kakaoLoginButton.onClick.AddListener(() => signupCanvas.SetActive(false));

            am = GameObject.Find("AppleManager").GetComponent<AppleManager>();
            appleLoginButton.onClick.AddListener(() => am.SignIN());

            dm = GameObject.Find("DeepLinkManager").GetComponent<DeepLinkManager>();
            dm.activatedAction += sw.CloseWebViewerCaller;
#elif UNITY_ANDROID
            kakaoLoginButton.onClick.AddListener(() => Application.OpenURL(appSetting.baseURL + "kakaoGetLogin"));

            appleLoginButton.gameObject.SetActive(false);
#endif

            // 로그인 건너뛰기
            skipLoginButton.onClick.AddListener(() => UserProfile.social_login_id = string.Empty);
            skipLoginButton.onClick.AddListener(() => sl.LoadSceneAsync("03 TestScene"));
// #if UNITY_IOS
//             am = GameObject.Find("AppleManager").GetComponent<AppleManager>();
//             appleLoginButton.onClick.AddListener(() => am.SignIN());
// #elif UNITY_ANDROID
//             appleLoginButton.gameObject.SetActive(false);
// #endif
        }
    }
}

