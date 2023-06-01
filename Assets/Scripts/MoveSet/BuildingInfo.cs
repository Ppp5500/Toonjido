using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInfo : MonoBehaviour
{
    public string address;
    public Vector3 navPosition;

    void Awake(){
        navPosition = transform.GetChild(0).GetComponent<Transform>().position;
    }
}
