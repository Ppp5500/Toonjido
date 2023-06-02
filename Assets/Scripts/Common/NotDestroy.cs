using UnityEngine;

public class NotDestroy : MonoBehaviour
{
    private void Awake()
    {
        Object.DontDestroyOnLoad(this);
    }
}
