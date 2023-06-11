using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonJido.UI{
    public class MenuCanvasControl : MonoBehaviour
    {
        public Canvas canvas;
        // public RectTransform sideRect;
        // Rect _safeArea;
        // Vector2 _minAnchor;
        // Vector2 _maxAnchor;
        // float _maxSafe;
        // float _minSafe;
        // float canvasSize;
        // float sideRectSize;
        // // Start is called before the first frame update
        // void Start()
        // {
        //     _safeArea = Screen.safeArea;
        //     canvasSize = canvas.GetComponent<RectTransform>().sizeDelta.x;
        //     sideRectSize = sideRect.sizeDelta.x;

        //     _minAnchor = _safeArea.position;
        //     _maxAnchor = _minAnchor + _safeArea.size;
        //     var temp = _minAnchor.x;

        //     _minAnchor.x /= Screen.width;
        //     _minAnchor.y /= Screen.height;
        //     _maxAnchor.x /= Screen.width;
        //     _maxAnchor.y /= Screen.height;

        //     _minSafe = canvasSize - sideRectSize;
        //     _maxSafe = _minSafe;
        // }

        // public void Open(){
        //     transform.position = canvas.transform.TransformPoint(new Vector2(-_maxSafe, 0));
        //     print(_maxSafe);
        // }

        // public void Close(){
        //     transform.position = canvas.transform.TransformPoint(new Vector2(-_minSafe, 0));
        //     print(_minSafe);
        // }

        public void Open(){
            canvas.gameObject.SetActive(true);
        }
    }
}

