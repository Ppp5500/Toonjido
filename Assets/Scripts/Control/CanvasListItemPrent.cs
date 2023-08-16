using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonJido.Control
{   
    public class CanvasListItemParent : MonoBehaviour
    {
        public virtual void SetActive(bool _value){
            if(_value is true)
            {
                BackKeyManager.GetInstance().AddActiveCanvasList(this.gameObject);
            }
            else
            {
                BackKeyManager.GetInstance().PopActiveCanvasList();
            }
            this.gameObject.SetActive(_value);
        }

        public virtual void OnDestroy() {
            BackKeyManager.GetInstance().PopActiveCanvasList();
        }
    }
}

