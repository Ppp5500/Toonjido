using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ToonJido.UI{
    public class SearchbarSelected : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI placeHolder;
        public void OnSelect(){
            placeHolder.text = string.Empty;
        }
    }
}
