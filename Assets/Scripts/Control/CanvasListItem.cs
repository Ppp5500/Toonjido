using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ToonJido.Control
{   
    public class CanvasListItem : CanvasListItemParent
    {
        [Tooltip("window가 지정된 경우에만 UI을 열때 애니메이션이 적용됩니다.")]
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