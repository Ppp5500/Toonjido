#if DEVELOPMENT
using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToonJido.Control;
using ToonJido.UI;
using UnityEngine;
using UnityEngine.UI;

public class DEVELOPMENTManager : MonoBehaviour
{
    public GameObject noticeManagerPref;
    public GameObject noticeCanvasPref;

    // Start is called before the first frame update
    void Start()
    {
        GameObject noticeManager = Instantiate(noticeManagerPref);
        var noticeManagerComp = noticeManager.GetComponent<NoticeManager>();

        GameObject noticeCanvas = Instantiate(noticeCanvasPref);

        noticeManagerComp.noticeCanvas = noticeCanvas.GetComponent<CanvasListItem>();
        noticeManagerComp.noticeArticle = noticeCanvas.transform.Find("Box").Find("Notice Article").GetComponent<TextMeshProUGUI>();
        noticeManagerComp.confirmButton = noticeCanvas.transform.Find("Box").Find("Button Area").Find("Confirm Button").GetComponent<Button>();
        noticeManagerComp.cancelButton = noticeCanvas.transform.Find("Box").Find("Button Area").Find("Cancel Button").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif