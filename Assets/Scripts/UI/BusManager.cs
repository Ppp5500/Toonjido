using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusManager : MonoBehaviour
{
    public Canvas busCanvas;
    public List<GameObject> busPref;
    public GameObject contentParent;
    private List<GameObject> busContainer = new List<GameObject>();

    public void OpenBusCanvas(int num){
        if(busContainer.Count > 0){
            foreach(var item in busContainer){
                Destroy(item);
            }
        }
        busCanvas.gameObject.SetActive(true);
        busContainer.Add(GameObject.Instantiate(busPref[num], contentParent.transform)); 
    }
}
