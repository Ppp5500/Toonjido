using System.Collections;
using UnityEngine;
using ToonJido.UI;

namespace ToonJido.Control
{
    public class GPSManager : MonoBehaviour
    {
        private float width;
        private float height;
        private float lat;
        private float lon;
        private float xPos;
        private float yPos;


        public Transform playerGPSLocation;
        public GameObject GPSMarker;
        private string gpsNotAvaliableMessage = "모든 기능은" + System.Environment.NewLine + "천안 원도심 지역 내에서만" + System.Environment.NewLine +"서비스가 가능합니다.";
        private NoticeManager noticeManager;

        void Awake()
        {
            width = (float)Screen.width / 2.0f;
            height = (float)Screen.height / 2.0f;

            lat = 0f;
            lon = 0f;
            xPos = 0f;
            yPos = 0f;
            CurrentControl.gpsStatus = GPSStatus.avaliable;
            // 원점 설정
            GPSEncoder.SetLocalOrigin(new Vector2(36.80926f, 127.14783f));
        }

        void Start()
        {
            noticeManager = NoticeManager.GetInstance();
            StartCoroutine(LocationCoroutine());
        }

        private void Update() {
#if DEVELOPMENT

#else
            if(CurrentControl.gpsStatus == GPSStatus.avaliable){
                GPSMarker.SetActive(true);
            }
            else{
                GPSMarker.SetActive(false);
            }
#endif

        }

#if DEVELOPMENT
        void OnGUI()
        {
            // GUI.backgroundColor = Color.yellow;
            // GUI.Box(new Rect(20, 20, 400, 400), "DebugCube");
            // // Compute a fontSize based on the size of the screen width.
            // GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            // GUI.Label(new Rect(20, 700, width, height * 0.25f), $"lat: {lat} lon: {lon} count:{count}");

            // GUI.Label(new Rect(20, 800, width, height * 0.25f), $"x: {xPos} y: {yPos}");

            // GUI.Label(new Rect(20, 900, width, height * 0.25f), $"message: {debugmessage} status:{CurrentControl.gpsStatus}");
        }
#endif
        private void FixedUpdate()
        {
#if DEVELOPMENT
            playerGPSLocation.position = new Vector3(26.3f, 0.5f, -27.7f);
#else
            if(CurrentControl.gpsStatus == GPSStatus.avaliable){
                playerGPSLocation.position = new Vector3(xPos, 0.5f, yPos);
            }
#endif
        }

        IEnumerator LocationCoroutine()
        {
            // Uncomment if you want to test with Unity Remote
            /*#if UNITY_EDITOR
                    yield return new WaitWhile(() => !UnityEditor.EditorApplication.isRemoteConnected);
                    yield return new WaitForSecondsRealtime(5f);
            #endif*/
#if UNITY_EDITOR
            // No permission handling needed in Editor
#elif UNITY_ANDROID

            if (
                !UnityEngine.Android.Permission.HasUserAuthorizedPermission(
                    UnityEngine.Android.Permission.FineLocation
                )
            )
            {
                UnityEngine.Android.Permission.RequestUserPermission(
                    UnityEngine.Android.Permission.FineLocation
                );
                while (
                    !UnityEngine.Android.Permission.HasUserAuthorizedPermission(
                        UnityEngine.Android.Permission.FineLocation
                    )
                )
                {
                    yield return null;
                }
            }

            // First, check if user has location service enabled
            if (!UnityEngine.Input.location.isEnabledByUser)
            {
                // TODO Failure
                // Debug.LogFormat("Android and Location not enabled");
                yield break;
            }

#elif UNITY_IOS
            if (!UnityEngine.Input.location.isEnabledByUser)
            {
                // TODO Failure
                // Debug.LogFormat("IOS and Location not enabled");
                yield break;
            }
#endif
            // Start service before querying location
            UnityEngine.Input.location.Start(0.1f, 0.1f);

            // Wait until service initializes
            int maxWait = 5;
            while (
                UnityEngine.Input.location.status == LocationServiceStatus.Initializing
                && maxWait > 0
            )
            {
                yield return new WaitForSecondsRealtime(1);
                maxWait--;
            }

            // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
#if UNITY_EDITOR
            int editorMaxWait = 5;
            while (
                UnityEngine.Input.location.status == LocationServiceStatus.Stopped
                && editorMaxWait > 0
            )
            {
                yield return new WaitForSecondsRealtime(1);
                editorMaxWait--;
            }
#endif

            // Service didn't initialize in 15 seconds
            if (maxWait < 1)
            {
                // TODO Failure
                Debug.LogFormat("Timed out");
                // debugmessage = "Timed out";
                noticeManager.ShowNoticeDefaultStyle(gpsNotAvaliableMessage);
                CurrentControl.gpsStatus = GPSStatus.invalid_timeout;
                yield break;
            }

            // Connection has failed
            if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
            {
                // TODO Failure
                Debug.LogFormat(
                    "Unable to determine device location. Failed with status {0}",
                    UnityEngine.Input.location.status
                );
                noticeManager.ShowNoticeDefaultStyle(gpsNotAvaliableMessage);
                CurrentControl.gpsStatus = GPSStatus.invalid_timeout;
                yield break;
            }
            else
            {
                lat = UnityEngine.Input.location.lastData.latitude;
                lon = UnityEngine.Input.location.lastData.longitude;

                if (lon < 127.14667760034673 | lon > 127.15273843675989 )
                {
                    print("lon is outrange");
                    // debugmessage = "lon is outrange";
                    noticeManager.ShowNoticeDefaultStyle(gpsNotAvaliableMessage);
                    CurrentControl.gpsStatus = GPSStatus.invalid_location;
                    yield break;
                }

                if (lat < 36.806334353989236 | lat > 36.811181607182505)
                {
                    print("lat is outrange");
                    // debugmessage = "lat is outrange";
                    noticeManager.ShowNoticeDefaultStyle(gpsNotAvaliableMessage);
                    CurrentControl.gpsStatus = GPSStatus.invalid_location;
                    yield break;
                }
                CurrentControl.gpsStatus = GPSStatus.avaliable;

                while (true)
                {
                    /*
                    Debug.LogFormat("Location service live. status {0}", UnityEngine.Input.location.status);
                    // Access granted and location value could be retrieved
                    Debug.LogFormat("Location: "
                        + UnityEngine.Input.location.lastData.latitude + " "
                        + UnityEngine.Input.location.lastData.longitude + " "
                        + UnityEngine.Input.location.lastData.altitude + " "
                        + UnityEngine.Input.location.lastData.horizontalAccuracy + " "
                        + UnityEngine.Input.location.lastData.timestamp);
                    */

                    lat = UnityEngine.Input.location.lastData.latitude;
                    lon = UnityEngine.Input.location.lastData.longitude;

                    var encodedValue = GPSEncoder.GPSToUCS(lat, lon);

                    xPos = encodedValue.x;
                    yPos = encodedValue.z;
                    // count++;
                    yield return new WaitForSeconds(.5f);
                }
            }
        }

        public void StopGPS()
        {
            // Stop service if there is no need to query location updates continuously
            UnityEngine.Input.location.Stop();

            StopCoroutine(LocationCoroutine());
        }

        public void StartGPSCoroutine()
        {
            StartCoroutine(LocationCoroutine());
        }
    }
}
