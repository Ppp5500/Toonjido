using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ToonJido.Control
{   
    public class CanvasListItem : CanvasListItemParent
    {
        [SerializeField] private RectTransform window;
        public override void SetActive(bool _value)
        {
            base.SetActive(_value);
            if(window != null){
                window.localScale = Vector3.zero;
                window.DOScale(Vector3.one, 0.3f);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}