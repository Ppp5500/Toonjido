using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaler : MonoBehaviour
{
    public Canvas canvas;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Rescale(){
        var dist = Vector3.Distance( canvas.transform.position, camera.transform.position);
        // canvas.transform.localScale = Vector3.one * dist / initial dist;
    }
}
