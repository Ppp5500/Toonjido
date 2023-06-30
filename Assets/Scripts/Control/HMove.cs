using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HMove : MonoBehaviour
{
    public float timing = 0;
    public float distFromCenter = 0;
    private RectTransform rect;
    float origin;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.anchoredPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        rect.anchoredPosition = new Vector2(origin, Mathf.Sin(Time.time + timing) * 30 + distFromCenter);
    }
}
