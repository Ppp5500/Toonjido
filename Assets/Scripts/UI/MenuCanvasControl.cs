using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToonJido.Control;

namespace ToonJido.UI{
    public class MenuCanvasControl : MonoBehaviour
    {
        public CanvasListItem canvas;

        public void Open(){
            canvas.SetActive(true);
        }
    }
}

