using System.Collections;
using UnityEngine;

public class PermissionManager : MonoBehaviour
{
    public GameObject failWindow;

    public void Request()
    {
        StartCoroutine(ClaimLocationPermission());
    }

    IEnumerator ClaimLocationPermission()
    {
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
            Debug.LogFormat("Android and Location not enabled");
            failWindow.SetActive(true);
            yield break;
        }

#elif UNITY_IOS
        if (!UnityEngine.Input.location.isEnabledByUser)
        {
            // TODO Failure
            Debug.LogFormat("IOS and Location not enabled");
            yield break;
        }
#endif
        yield return null;
    }
}
