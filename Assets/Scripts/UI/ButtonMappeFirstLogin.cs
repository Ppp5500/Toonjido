using UnityEngine;
using UnityEngine.UI;
using ToonJido.Login;

namespace ToonJido.UI{
    public class ButtonMappeFirstLogin : MonoBehaviour
    {
        private SceneLoaderSingleton sl;
        private LoginManager lm;

        [SerializeField] private Button tutorialCheckButton;
        [SerializeField] private Button kakaoLoginButton;

        void Start()
        {
            sl = GameObject.Find("SceneLoader").GetComponent<SceneLoaderSingleton>();
            tutorialCheckButton.onClick.AddListener(() => sl.LoadSceneAsync("03 TestScene"));

            lm = GameObject.Find("LoginManager").GetComponent<LoginManager>();
            kakaoLoginButton.onClick.AddListener(() => lm.OpenKakaoLogin());
        }
    }
}
