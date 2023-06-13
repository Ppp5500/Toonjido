using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityMove : MonoBehaviour
{
    public float moveSpeed;
    private RectTransform rect;
    Vector2 origin;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rect.anchoredPosition = new Vector2(origin.x += Time.deltaTime * moveSpeed, origin.y);
    }
}