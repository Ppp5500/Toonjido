using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGPosition : MonoBehaviour
{
        Rect _safeArea; 
        Vector2 _minAnchor;
        Vector2 _maxAnchor;

        RectTransform rect;

        void Start() {
            _safeArea = Screen.safeArea;
            print(_safeArea);
            _minAnchor = _safeArea.position;
            _maxAnchor = _minAnchor + _safeArea.size;

            print(_minAnchor);
            print(_maxAnchor);

            _minAnchor.x /= Screen.width;
            _minAnchor.y /= Screen.height;
            _maxAnchor.x /= Screen.width;
            _maxAnchor.y /= Screen.height;

            print(_minAnchor.x);
            print(_minAnchor.y);

            rect = GetComponent<RectTransform>();
            rect.anchorMin = _minAnchor;
            rect.anchorMax = _maxAnchor;
        }
}
