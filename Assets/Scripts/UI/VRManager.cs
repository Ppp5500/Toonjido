using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToonJido.Search;
using ToonJido.Control;

namespace ToonJido.UI{
    public class VRManager : MonoBehaviour
    {
        [SerializeField] GameObject camTarget;
        [SerializeField] GameObject eyelevelPlayer;
        [SerializeField] Toggle vrToggle;
        [SerializeField] GameObject vrObjects;
        private BuildingManager buildingManager;
        // Start is called before the first frame update
        void Start()
        {
            buildingManager = BuildingManager.buildingManager;
            vrToggle.onValueChanged.AddListener((x) => ToggleOnOff(x));
        }

        private void ToggleOnOff(bool input){
            vrObjects.SetActive(input);
        }

        public void OpenVrLink(string url){
            Application.OpenURL(url);
        }

        public void MoveToFront(Transform target){
            StartCoroutine(FTS(camTarget, target, eyelevelPlayer));
        }

        IEnumerator FTS(GameObject _camTarget, Transform target, GameObject _player){
            float t = 0.0f;
            Vector3 startPosition = _camTarget.transform.position;

            while(t < 1){
                t += Time.deltaTime/0.5f;
                _camTarget.transform.position = Vector3.Lerp(startPosition, target.transform.position, t);
                yield return null;
            }

            eyelevelPlayer.transform.position = _camTarget.transform.position;
            eyelevelPlayer.transform.rotation = target.transform.rotation;
            CurrentControl.ChangeToEyelevel();
        }
    }
}

