using UnityEngine;
using UnityEngine.UI;

namespace ToonJido.Login
{
    public class LoginManager : MonoBehaviour
    {
        private Button kakaoLoginButton;

        private void Awake()
        {
            kakaoLoginButton = GameObject.Find("KakaoLoginButton").GetComponent<Button>();
        }
        // Start is called before the first frame update
        void Start()
        {
            kakaoLoginButton.onClick.AddListener(() =>
            {
                Application.OpenURL(appSetting.baseURL + "kakaoGetLogin");
            });
        }
    }
}

