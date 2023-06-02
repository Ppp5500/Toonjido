using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ContentScaler : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollRectTransform;
    [SerializeField] private RectTransform _contentRectTransform;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private float minScrollHeight;
    [SerializeField] private float maxScrollHeight;

    private void Awake()
    {
        if (!_contentRectTransform) _contentRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Get the preferred Height the text component would require in order to 
        // display all available text
        var desiredHeight = _text.preferredHeight;

        // actually make your text have that height
        var textSizeDelta = _text.rectTransform.sizeDelta;
        textSizeDelta.y = desiredHeight;
        _text.rectTransform.sizeDelta = textSizeDelta;

        // Then make the content have the same height
        var contentSizeDelta = _contentRectTransform.sizeDelta;
        contentSizeDelta.y = desiredHeight;
        _contentRectTransform.sizeDelta = contentSizeDelta;

        var scrollSizeDelta = _scrollRectTransform.sizeDelta;
        scrollSizeDelta.y = Mathf.Clamp(desiredHeight, minScrollHeight, maxScrollHeight);
        _scrollRectTransform.sizeDelta = scrollSizeDelta;
    }
}
