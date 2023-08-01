using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonJido.Common;
using ToonJido.Data.Model;
using TMPro;
using ToonJido.Control;

namespace ToonJido.Game{
    public class FortuneWindowManager : MonoBehaviour
    {
        [SerializeField] CanvasListItem windowObject;
        [SerializeField] TextMeshProUGUI article;

        List<Fortune> fortunes = new();

        // singleton
        static FortuneWindowManager instance;

        void Awake() {
            if(instance == null){
                instance = this;
            }
            else{
                Destroy(this);
            }
        }

        public static FortuneWindowManager GetInstance(){
            return instance;
        }
        // Start is called before the first frame update
        void Start()
        {
            using(FortuneFileReader reader = new()){
                fortunes = reader.Load();
            }
        }

        public void ShowFortune(){
            print("운세 뜸!");
            windowObject.SetActive(true);
            article.text = SelectRandom();
        }

        private string SelectRandom(){
            int randomIndex = Random.Range(0, fortunes.Count);
            return fortunes[randomIndex].desc;
        }
    }
}

