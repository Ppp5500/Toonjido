using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;

using ToonJido.Control;

using UnityEngine;
using UnityEngine.UI;

namespace ToonJido.UI{
    public class NoticeManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public CanvasListItem noticeCanvas;
        public TextMeshProUGUI noticeArticle;
        public Button confirmButton;
        public Button cancelButton;

        [Space(10)]
        [Header("Test")]
        [SerializeField] private CanvasListItem noticeCanvas2;
        TextMeshProUGUI currArticle;
        Button currConfirmButton;
        Button currCancleButton;

        
        // 싱글톤 객체
        private static NoticeManager instance;

        void Awake() {
            if(instance is null){
                instance = this;
            }
            else{
                Destroy(this);
            }

            if(noticeCanvas != null){
                DontDestroyOnLoad(noticeCanvas);
            }
        }

#if DEVELOPMENT
        void OnGUI() {
            if(GUI.Button(new Rect(250, 250, 150, 150), "Init Canvas!")){
                InitCanvas("test!");
            };
        }
#endif

        public static NoticeManager GetInstance(){
            return instance;
        }

        public void ShowNotice(string text){
            noticeArticle.text = text;
            noticeCanvas.SetActive(true);
        }

        /// <summary>
        /// Show notice canvas as default style(message and one confirm button that for close canvas)
        /// </summary>
        /// <param name="text"></param>
        public void ShowNoticeDefaultStyle(string text){
            DisableCancelButton();
            SetConfirmButtonDefault();
            noticeArticle.text = text;
            noticeCanvas.SetActive(true);
        }

        public NoticeManager DisableCancelButton(){
            cancelButton.gameObject.SetActive(false);
            return this;
        }

        /// <summary>
        /// Set Confirm Button Onclick to Close Notice Canvas
        /// </summary>
        public NoticeManager SetConfirmButtonDefault(){
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener( () => { noticeCanvas.SetActive(false); });
            return this;
        }

        public NoticeManager SetConfirmButton(Action action){
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(() => action());
            return this;
        }

        /// <summary>
        /// Set Cancel Button Onclick to Close Notice Canvas
        /// </summary>
        public NoticeManager SetCancelButtonDefault(){
            if(cancelButton.gameObject.activeSelf is false) cancelButton.gameObject.SetActive(true); 
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener( () => { noticeCanvas.SetActive(false); });
            return this;
        }

        public NoticeManager SetCancelButton(Action action){
            if(cancelButton.gameObject.activeSelf is false) cancelButton.gameObject.SetActive(true); 
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() => action());
            return this;
        }

        public void CloseCanvas(){
            noticeCanvas.SetActive(false);
        }


#region Method Ver 2
        /// <summary>
        /// Show notice canvas as default style(message and one confirm button that for close canvas)
        /// </summary>
        /// <param name="text"></param>
        public NoticeManager InitCanvas(string text){
            CanvasListItem canv = Instantiate(noticeCanvas2);
            currArticle = canv.transform.Find("Box").Find("Notice Article").GetComponent<TextMeshProUGUI>();
            currConfirmButton = canv.transform.Find("Box").Find("Button Area").Find("Confirm Button").GetComponent<Button>();
            currCancleButton = canv.transform.Find("Box").Find("Button Area").Find("Cancel Button").GetComponent<Button>();

            currCancleButton.onClick.AddListener(() => {
                            Destroy(canv.gameObject);
            });
            currArticle.text = "text";

            canv.SetActive(true);

            return this;
        }

        public NoticeManager DisableCancelButtonVer2(){
            currCancleButton.gameObject.SetActive(false);
            return this;
        }

        /// <summary>
        /// Set Confirm Button Onclick to Close Notice Canvas
        /// </summary>
        public NoticeManager SetConfirmButtonDefaultVer2(){
            currConfirmButton.onClick.RemoveAllListeners();
            currConfirmButton.onClick.AddListener( () => { noticeCanvas.SetActive(false); });
            return this;
        }

        public NoticeManager SetConfirmButtonVer2(Action action){
            currConfirmButton.onClick.RemoveAllListeners();
            currConfirmButton.onClick.AddListener(() => action());
            return this;
        }

        /// <summary>
        /// Set Cancel Button Onclick to Close Notice Canvas
        /// </summary>
        public NoticeManager SetCancelButtonDefaultVer2(){
            if(currCancleButton.gameObject.activeSelf is false) currCancleButton.gameObject.SetActive(true); 
            currCancleButton.onClick.RemoveAllListeners();
            currCancleButton.onClick.AddListener( () => { noticeCanvas.SetActive(false); });
            return this;
        }

        public NoticeManager SetCancelButtonVer2(Action action){
            if(currCancleButton.gameObject.activeSelf is false) currCancleButton.gameObject.SetActive(true); 
            currCancleButton.onClick.RemoveAllListeners();
            currCancleButton.onClick.AddListener(() => action());
            return this;
        }
#endregion
    }
}

