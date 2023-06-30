using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonJido.UI;

namespace ToonJido.Control{
    public class BackKeyManager : MonoBehaviour
    {
        private static BackKeyManager instance;
        private Stack<GameObject> activeCanvases = new Stack<GameObject>();
        [SerializeField] private UIDrag drag;

        private void Awake() {
            if(instance == null){
                instance = this;
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
                if(activeCanvases.Count > 0){
                    PopActiveCanvasList();
                }
                else{
                    drag.BackKeyInput();
                }
            }
        }
#endif

        public void AddActiveCanvasList(GameObject input){
            input.SetActive(true);
            activeCanvases.Push(input);
        }

        public void PopActiveCanvasList(){
            activeCanvases.Pop().SetActive(false);
        }

    }
}

