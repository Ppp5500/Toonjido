using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ToonJido.UI{
    public class NoticeManager : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject noticeCanvas;
        [SerializeField] private TextMeshProUGUI noticeArticle;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        // 싱글톤 객체
        private static NoticeManager instance;

        void Awake() {
            if(instance is null){
                instance = this;
            }

            DontDestroyOnLoad(noticeCanvas);
        }

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

        public void DisableCancelButton(){
            cancelButton.gameObject.SetActive(false); 
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
    }
}

