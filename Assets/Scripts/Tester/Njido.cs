using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Njido : MonoBehaviour
{
    public RawImage mapRawImage;

    [Header("맵 정보 입력")]
    public string strBaseURL;
    public string latitude;
    public string longitude;
    public int level;
    public int mapWidth;
    public int mapHeight;
    public string strAPIkey;
    public string secretKey;

    private void Start() {
        mapRawImage = GetComponent<RawImage>();
        StartCoroutine(MapLoader());
    }

    IEnumerator MapLoader(){
        string str = strBaseURL + "?w=" + mapWidth.ToString() + "&h=" + mapHeight.ToString() + "&center="+longitude.ToString() +"," + latitude + "&level=" + level.ToString();

        print(str);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(str);

        request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", strAPIkey);
        request.SetRequestHeader("X-NCP-APIGW-API-KEY", secretKey);

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError){
            print("error");
        }
        else{
            mapRawImage.texture = DownloadHandlerTexture.GetContent(request);
        }
    }
}
