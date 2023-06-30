using UnityEngine;
using UnityEngine.UI;
using ToonJido.Login;
using ToonJido.UI;
using static appSetting;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;

public class AccountChecker : MonoBehaviour
{
    public Button logoutButton;
    public Button withdrawalButton;
    public Button zzimlistButton;

    // common managers
    private NoticeManager noticeManager;

    // Start is called before the first frame update
    void Start()
    {
        noticeManager = NoticeManager.GetInstance();
        //JWTTest();
        // GenerateAppStoreJwtToken("MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgqLt9ltnpdwEE36ywY3FNzOFT1T7Fx8HFQXPrZMg9XpCgCgYIKoZIzj0DAQehRANCAARV/UD4kBLJcApsbVXLnw55ZXl30VwBRy0Sd5MmBXKwB0NUfdC1w2h/8I1QPxS5EO6MkS57xGokt+2/Kt+JRvsJ");
        if(UserProfile.social_login_id == string.Empty)
        {
            NoAccount();
            HideLogoutAndWithdrawalButton();
        }
        else
        {

        }
    }

    public void NoAccount(){
        zzimlistButton.onClick.RemoveAllListeners();
        zzimlistButton.onClick.AddListener(() => noticeManager.ShowNoticeDefaultStyle("찜 기능은 로그인 후 이용가능 합니다."));
    }

    public void HideLogoutAndWithdrawalButton(){
        logoutButton.gameObject.SetActive(false);
    }

    public void SetLogoutButton(){
        logoutButton.gameObject.SetActive(true);

        if(UserProfile.curr_id_type == IdType.apple)
        {
            PlayerPrefs.DeleteKey(AppleUserIdKey);
        }
        else if(UserProfile.curr_id_type == IdType.kakao)
        {
            PlayerPrefs.DeleteKey(KakaoUserIdKey);
        }
    }

    public void SetWithdrawalButton(){
        withdrawalButton.gameObject.SetActive(true);
        withdrawalButton.onClick.AddListener(
            () =>
            {
                noticeManager.SetCancelButtonDefault()
                                .SetConfirmButton(
                                    () => 
                                    {
                                        var url = baseURL + "deleteAccount/" + UserProfile.social_login_id;
                                        if(UserProfile.curr_id_type == IdType.apple)
                                        {

                                        }
                                        else if(UserProfile.curr_id_type == IdType.kakao)
                                        {

                                        }
                                    })
                                    .ShowNotice("사용자 정보가 모두 삭제 됩니다. 계속 하시겠습니까?");
            }
        );
    }

    // public string GenerateAppStoreJwtToken(string privateKey)
    // {
    //     var header = new Dictionary<string, object>()
    //     {
    //         { "alg", "ES256" },
    //         { "kid", "MY_VALUE" },
    //         { "typ", "JWT" }
    //     };
        
    //     var scope = new string[1] { "GET /v1/apps?filter[platform]=IOS" };
    //     var payload = new Dictionary<string, object>
    //     {
    //         { "iss", "MY_VALUE" },
    //         { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
    //         { "exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds() },
    //         { "aud", "appstoreconnect-v1" },
    //         { "scope", scope }
    //     };
        
    //     CngKey key = CngKey.Import(Convert.FromBase64String(privateKey), CngKeyBlobFormat.Pkcs8PrivateBlob);

    //     string token = JWT.Encode(payload, key, JwsAlgorithm.ES256, header);

    //     print(token);
    //     return token;
    // }

    // public void JWTTest(){
    //     var token = new JwtBuilder()
    //                   .WithAlgorithm(new ES256Algorithm(ECDsa.Create(), GetPrivateKey()))
    //                   .AddHeader("alg", "ES256")
    //                   .AddHeader("kid", "MUU9ZCSM6Y")
    //                   .AddClaim("iss", "B443Z6CA7T")
    //                   .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
    //                   .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
    //                   .AddClaim("aud", "https://appleid.apple.com")
    //                   .AddClaim("sub", "com.roundstar.ToonJido")
    //                   .Encode();
    //     print(token);
    // }

    // public string GenerateAppleSecret(string client_id, string TeamId, string kid, string key)
    // {
    //     var now = DateTimeOffset.Now.AddDays(-1);
    //     ReadOnlySpan<byte> keyAsSpan = Convert.FromBase64String(key);
    //     var prvKey = ECDsa.Create();
    //     prvKey.ImportPkcs8PrivateKey(keyAsSpan,out var read);
    //     return new JwtBuilder()
    //         .WithAlgorithm(new ES256Algorithm(ECDsa.Create(), prvKey))
    //         .AddHeader("kid", kid) 
    //         .ExpirationTime(now.AddDays(5).UtcDateTime)
    //         .IssuedAt(now.UtcDateTime)
    //         .Issuer(TeamId)
    //         .Audience("https://appleid.apple.com")
    //         .Subject(client_id)
    //         .Encode();
    // }

    public ECDsa GetPrivateKey(){
        // var data = Resources.Load<TextAsset>("AuthKey_MUU9ZCSM6Y");
        // string txt = data.text;
        // print(txt);
        // var privateKeyContent = File.ReadAllText("AuthKey_MUU9ZCSM6Y.p8");
        // var privateKeyList = privateKeyContent.Split('\n').ToList();
        // var privateKey = privateKeyList.Where((s, i) => i != 0 && i != privateKeyList.Count - 1)
        //                            .Aggregate((agg, s) => agg + s);
        var privateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgqLt9ltnpdwEE36ywY3FNzOFT1T7Fx8HFQXPrZMg9XpCgCgYIKoZIzj0DAQehRANCAARV/UD4kBLJcApsbVXLnw55ZXl30VwBRy0Sd5MmBXKwB0NUfdC1w2h/8I1QPxS5EO6MkS57xGokt+2/Kt+JRvsJ";
        ReadOnlySpan<byte> keyAsSpan = Convert.FromBase64String(privateKey);
        var prvKey = ECDsa.Create();
        prvKey.ImportPkcs8PrivateKey(keyAsSpan, out var read);

        return prvKey;
    }
}