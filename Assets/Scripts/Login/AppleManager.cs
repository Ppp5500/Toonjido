using System.Collections.Generic;
using System.Text;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using AppleAuth.Extensions;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using ToonJido.Common;
using ToonJido.Login;
using static appSetting;
using System;
using ToonJido.Data.Saver;
using ToonJido.Data.Model;

public class AppleManager : MonoBehaviour
{
    private IAppleAuthManager appleAuthManager;

    private float width;
    private float height;
    public string debugText = string.Empty;

    private HttpClient client;

    private void Awake() {
        width = Screen.width * 0.5f;
        height = Screen.height * 0.5f;
    }

    void Start()
    {
        // If the current platform is supported
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
        else{
            Debug.Log("apple sign in not support!");
            Destroy(this.gameObject);
        }

        client = HttpClientProvider.GetHttpClient();
    }

    void Update()
    {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }
    }

    // private void OnGUI() {
    //     GUI.backgroundColor = Color.yellow;
    //     GUI.Box(new Rect(20, 200, width, height), "DebugCube");

    //     // Compute a fontSize based on the size of the screen width.
    //     GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

    //     GUI.Label(
    //         new Rect(20, 200, width, height),
    //         $"apple :{debugText}"
    //     );
    // }

    public void SignIN(){
        debugText += "\nwe are in first line of sign in";
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);
        debugText = "\nwe are in second line of sign in";

        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            async credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;
                //debugText += "\nwe are in second line of login";

                if (appleIdCredential != null)
                {
                    //debugText += "\nwe are in third line of login";

                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;
                    //debugText += "\nwe are in 4 line of login";

                    // Email (Received ONLY in the first login)
                    var email = appleIdCredential.Email;
                    //debugText += "\nwe are in 5 line of login";

                    // Full name (Received ONLY in the first login)
                    var fullName = appleIdCredential.FullName;
                    //debugText += "\nwe are in 6 line of login";

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                        appleIdCredential.IdentityToken,
                        0,
                        appleIdCredential.IdentityToken.Length);
                    //debugText += "\nwe are in 7 line of login";

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);
                    //debugText += "\nwe are in 8 line of login";

                    // And now you have all the information to create/login a user in your system
                    debugText += "\nauth code: " + authorizationCode;
                    //debugText += "\nuser: " + userId;
                    //debugText += "\nemail: " + email;
                    //debugText += "\ntoken: " + identityToken;
                    //debugText += "\nauth code: "+ authorizationCode;
                    Debug.Log("id: "+ userId);
                    Debug.Log("token: "+ identityToken);
                    Debug.Log("auth code: "+ authorizationCode);


                    if(email is null)
                    {
                        email = string.Empty;
                    }
                    bool isSuccess = await APIServerSigninRequest(userId, email);
                    Debug.Log("create Account is success?: " + isSuccess);
                    if(isSuccess)
                    {
                        // 다음 씬으로
                        try{
                            PlayerPrefs.SetString(AppleUserIdKey, userId);
                            PlayerPrefs.Save();
                            print("save success!");
                        }
                        catch(Exception ex)
                        {
                            Debug.Log("save prefs went wrong!");
                            Debug.Log("what cause save problem: " + ex.Message);
                        }
                        Debug.Log("appleid: "+ userId);
                        print("saved id: " + PlayerPrefs.GetString(AppleUserIdKey));
                        print("is id saved?:" + PlayerPrefs.HasKey(AppleUserIdKey));
                        // PlayerPrefs.SetString("AppleUserEmail", email);
                        UserProfile.social_login_id = userId;
                        UserProfile.curr_id_type = IdType.apple;

                        using (PlayerDataSaver saver = new())
                        {
                            User user = new(){
                                user_social_id = userId,
                                idType = IdType.apple
                            };
                            await saver.SavePlayerInfo(user);
                        }
                        SceneLoaderSingleton.instance.LoadSceneAsync("03 TestScene");
                    }
                    else
                    {
                        Debug.Log("Create Account fail");
                    }
                }
            },
            error =>
            {
                // Something went wrong
                debugText += "something went wrong";

                var authorizationErrorCode = error.GetAuthorizationErrorCode();
            }
        );
    }

    public void SignOUT(){
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            async credential =>
            {
                // Obtained credential, cast it to IAppleIDCredential
                var appleIdCredential = credential as IAppleIDCredential;

                if (appleIdCredential != null)
                {
                    // Apple User ID
                    // You should save the user ID somewhere in the device
                    var userId = appleIdCredential.User;

                    // Identity token
                    var identityToken = Encoding.UTF8.GetString(
                        appleIdCredential.IdentityToken,
                        0,
                        appleIdCredential.IdentityToken.Length);

                    // Authorization code
                    var authorizationCode = Encoding.UTF8.GetString(
                        appleIdCredential.AuthorizationCode,
                        0,
                        appleIdCredential.AuthorizationCode.Length);

                    // And now you have all the information to create/login a user in your system
                    // debugText += "\nauth code: " + authorizationCode;
                    // debugText += "\nuser: " + userId;
                    // debugText += "\nemail: " + email;

                    await AppleServerSignoutRequest(userId, identityToken);
                }
            },
            error =>
            {
                // Something went wrong
                debugText += "something went wrong";
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
            }
        );
    }

    public async Task<bool> APIServerSigninRequest(string id, string email){
        debugText += "now we are in request";
        var url = appSetting.baseURL + "apple_login/";
        var values = new Dictionary<string, string>{
            {"social_login_id", id},
            {"email", email}
        };

        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AppleServerSignoutRequest(string id, string token){
        debugText += "now we are in request";
        var url = appSetting.baseURL + "apple_login/";
        var values = new Dictionary<string, string>{
            {"social_login_id", id},
            {"token", token}
        };

        var data = new FormUrlEncodedContent(values);
        var response = await client.PostAsync(url, data);

        return response.IsSuccessStatusCode;
    }
}
