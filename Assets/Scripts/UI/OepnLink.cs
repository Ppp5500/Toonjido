using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OepnLink : MonoBehaviour
{
    public void GoThere(string url){
        Application.OpenURL(url);
    }
}
