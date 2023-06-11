using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ToonJido.UI{
    public class NoticeManager : MonoBehaviour
    {
        [SerializeField] private GameObject noticeCanvas;
        [SerializeField] private TextMeshProUGUI noticeArticle;

        public static NoticeManager instance;

        void Awake() {
            if(instance is null){
                instance = this;
            }
        }

        public NoticeManager GetInstance(){
            return instance;
        }

        public void ShowNotice(string text){
            noticeArticle.text = text;
            noticeCanvas.SetActive(true);
        }
    }
}

