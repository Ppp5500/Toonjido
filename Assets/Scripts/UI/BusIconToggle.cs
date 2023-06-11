using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusIconToggle : MonoBehaviour
{
    Toggle toggle;
    [SerializeField] GameObject busIconCanvas;
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener( delegate{ BusIconCanvasOnOff();});
    }

    public void BusIconCanvasOnOff(){
        if(toggle.isOn){
            busIconCanvas.SetActive(true);
        }
        else{
            busIconCanvas.SetActive(false);
        }
    }
}
