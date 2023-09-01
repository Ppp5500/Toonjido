using UnityEngine;
using ToonJido.Login;

namespace ToonJido.Test
{
    public class ApplicationInformer : MonoBehaviour
    {
#if DEVELOPMENT
        private Vector3 position;
        private float width;
        private float height;


        void Awake()
        {
            width = (float)Screen.width / 2.0f;
            height = (float)Screen.height / 2.0f;

            // Position used for the cube.
            position = new Vector3(0.0f, 0.0f, 0.0f);
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            // Compute a fontSize based on the size of the screen width.
            GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            GUI.Label(
                new Rect(20, 60, width, height * 0.25f),
                $"platform:Editor\nver:{Application.version}"
            );

            GUI.Label(
                new Rect(20, 160, width, height * 0.25f),
                $"s id:{UserProfile.social_login_id}"
            );
        }
#elif UNITY_ANDROID
        void OnGUI()
        {
            // Compute a fontSize based on the size of the screen width.
            GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            GUI.Label(new Rect(20, 20, width, height * 0.25f), "platform:Android");
            GUI.Label(
                new Rect(20, 160, width, height * 0.25f),
                $"s id:{UserProfile.social_login_id}"
            );
        }
#elif UNITY_IOS
        void OnGUI()
        {
            //Compute a fontSize based on the size of the screen width.
            GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            GUI.Label(new Rect(20, 20, width, height * 0.25f), "platform:iOS");
            GUI.Label(
                new Rect(20, 160, width, height * 0.25f),
                $"s id:{UserProfile.social_login_id}"
            );
        }
#endif
#endif
    }
}
