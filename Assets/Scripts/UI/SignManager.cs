using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using ToonJido.Search;

namespace ToonJido.UI{
    public class SignManager : MonoBehaviour
    {
        [SerializeField] List<GameObject> signParents;
        [SerializeField] List<Toggle> toggles;

        List<SignObject> signs = new();
        bool isOn;
        public float FOV_Size_Aspect;

        void Start() {
            foreach(var signParent in signParents){
                List<SignObject> signs = new();
                signParent.GetComponentsInChildren<SignObject>(signs);

                foreach(var sign in signs){
                    sign.gameObject.GetComponent<Button>().onClick.AddListener(
                        () => SearchManager.instance.OpenDetailCanvas(sign.storeName)
                    );
                }
            }

            for(int i = 0; i < toggles.Count - 1; i++){
                var currObj = signParents[i];
                toggles[i].onValueChanged.AddListener(
                    (value) => 
                    {
                        currObj.SetActive(value);
                        isOn = value;
                        if(value){
                            currObj.GetComponentsInChildren<SignObject>(signs);
                        }
                        else{
                            signs.RemoveRange(0,signs.Count);
                        }    
                    }
                );
            }
        }

        void Update() {
            if(signs.Count > 0){
                foreach(var sign in signs){
                    sign.ReSize(Camera.main.fieldOfView, FOV_Size_Aspect);
                }
            }
        }
    }
}

