using System.Collections;
using UnityEngine;

namespace ToonJido.Control
{
    public enum GPSStatus
    {
        avaliable,
        invalid_timeout,
        invalid_location
    }

    public class GPSManager : MonoBehaviour
    {
        private float width;
        private float height;
        private float lat;
        private float lon;
        private float xPos;
        private float yPos;
        private int count;
        private string debugmessage;
        private GPSStatus status;

        public Transform playerGPSLocation;

        void Awake()
        {
            width = (float)Screen.width / 2.0f;
            height = (float)Screen.height / 2.0f;

            lat = 0f;
            lon = 0f;
            xPos = 0f;
            yPos = 0f;
            count = 0;
            status = GPSStatus.avaliable;

            playerGPSLocation = transform;

            // 그린스타트업타운
            // GPSEncoder.SetLocalOrigin(new Vector2(36.80939f, 127.1443f));

            // 천안역
            GPSEncoder.SetLocalOrigin(new Vector2(36.80926f, 127.14783f));
        }

        void Start()
        {
            StartCoroutine(LocationCoroutine());

        }

#if DEVELOPMENT
        void OnGUI()
        {
            GUI.backgroundColor = Color.yellow;
            GUI.Box(new Rect(20, 20, 400, 400), "DebugCube");
            // Compute a fontSize based on the size of the screen width.
            GUI.skin.label.fontSize = (int)(Screen.width / 25.0f);

            //GUI.Label(new Rect(20, 100, width, height * 0.25f),
            //     $"lat: {lat} lon: {lon}");

            //GUI.Label(new Rect(20, 200, width, height * 0.25f),
            //     $"x: {xPos} lon: {yPos}");

            GUI.Label(new Rect(20, 300, width, height * 0.25f),
                 $"message: {debugmessage}");
        }
#endif

        private void FixedUpdate()
        {
            if (playerGPSLocation != null)
            {
                playerGPSLocation.position = new Vector3(xPos, 0.5f, yPos);
            }
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

        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.FineLocation);
            while (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.FineLocation))
            {
                yield return null;
            }
        }

        //권한 요청인데 s7에서 작동 안 됨
        /*
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.CoarseLocation)) {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.CoarseLocation);
        }
        */

        // First, check if user has location service enabled
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("Android and Location not enabled");
            yield break;
        }

#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser) {
            // TODO Failure
            Debug.LogFormat("IOS and Location not enabled");
            yield break;
        }
#endif
            // Start service before querying location
            UnityEngine.Input.location.Start(0.1f, 0.1f);

            // Wait until service initializes
            int maxWait = 15;
            while (UnityEngine.Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                maxWait--;
            }

            // Editor has a bug which doesn't set the service status to Initializing. So extra wait in Editor.
#if UNITY_EDITOR
            int editorMaxWait = 15;
            while (UnityEngine.Input.location.status == LocationServiceStatus.Stopped && editorMaxWait > 0)
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
                status = GPSStatus.invalid_timeout;
                yield break;
            }

            // Connection has failed
            if (UnityEngine.Input.location.status != LocationServiceStatus.Running)
            {
                // TODO Failure
                Debug.LogFormat("Unable to determine device location. Failed with status {0}", UnityEngine.Input.location.status);
                yield break;
            }
            else
            {
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

                    if(lon < 127.14559498942343 | lon > 127.14559498942343)
                    {
                        print("lon is outrange");
                        debugmessage = "lon is outrange";
                        yield break;
                    }

                    if(lat > 36.80627972193478 |lat < 36.813149916958146)
                    {
                        print("lat is outrange");
                        debugmessage = "lat is outrange";
                        yield break;
                    }

                    var encodedValue = GPSEncoder.GPSToUCS(lat, lon);

                    xPos = encodedValue.x;
                    yPos = encodedValue.z;
                    count++;
                    yield return new WaitForSeconds(.1f);
                }
            }
        }

        public void StopGPS()
        {
            // Stop service if there is no need to query location updates continuously
            UnityEngine.Input.location.Stop();

            StopCoroutine(LocationCoroutine());
        }

        public void LocationAvailableCheck()
        {

        }
    }
}