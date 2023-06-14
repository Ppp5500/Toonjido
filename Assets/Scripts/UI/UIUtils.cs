using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtils
{
    private EventSystem eventSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 터치가 UI위에서 이루어지는지 검사
    public bool IsPointerOverUI(int fingerId)
    {
        return (
            eventSystem.IsPointerOverGameObject(fingerId)
            && eventSystem.currentSelectedGameObject != null
        );
    }
}
