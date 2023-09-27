using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonJido.UI;
using UnityEngine.SceneManagement;
using System;

namespace ToonJido.Control{
    public class BackKeyManager : MonoBehaviour
    {
        private static BackKeyManager instance;
        private Stack<GameObject> activeCanvases = new Stack<GameObject>();
        private UIDrag drag;

        private void Awake() {
            if(instance == null){
                instance = this;
            }
        }

        void Start(){
            SceneManager.sceneLoaded += ModalCheck;
        }

        private void ModalCheck(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.name == "03 MainScene"){
                drag = GameObject.Find("MainUICanvas ver2").transform.Find("Modal").GetComponent<UIDrag>();
            }
        }

        public static BackKeyManager GetInstance(){
            return instance;
        }

#if UNITY_ANDROID
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape)){
                if(activeCanvases.Count > 0)
                {
                    if(CurrentControl.state == State.Weather)
                    {
                        CurrentControl.ChangeToLastState();
                    }
                    else
                    {
                        PopActiveCanvasList();
                    }
                }
                else if(drag.isModalUp)
                {
                    drag.Down();
                }
                else
                {
                    NoticeManager.GetInstance()
                        .SetCancelButtonDefault()
                        .SetConfirmButton(() => Application.Quit())
                        .ShowNotice("어플리케이션을 종료하시겠습니까?");
                }

            }
        }
#endif

        public void AddActiveCanvasList(GameObject _input){
            _input.SetActive(true);
            activeCanvases.Push(_input);
        }

        public void PopActiveCanvasList(){
            activeCanvases.Pop().SetActive(false);
        }

        public void PopActiveCanvasList(GameObject _input){
            if(activeCanvases.Contains(_input))
            {
                activeCanvases.Pop().SetActive(false);
            }
            else
            {
                print("something went wrong");
            }

        }

        public GameObject GetPeek(){
            return activeCanvases.Peek();
        }
    }
}

