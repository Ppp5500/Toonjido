using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionDisplayManager : MonoBehaviour
{

    public TextMeshProUGUI versionText;
    // Start is called before the first frame update
    void Start()
    {
        DisplayVerison();
    }

    void DisplayVerison(){
        versionText.text = Application.version;
    }
}
