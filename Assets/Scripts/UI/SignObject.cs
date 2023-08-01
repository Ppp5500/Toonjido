using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ToonJido.UI{
    public enum Category{
    Food,
    Fassion,
    Staying,
    Bus,
    Infra,
    Medical,
    Park

}
    [RequireComponent(typeof(Image))]
    public class SignObject : MonoBehaviour
    {
        public Category category;
        public bool isStore;
        [HideInInspector] public string storeName;
        Image myImage;
        TextMeshProUGUI myText;
        // Start is called before the first frame update
        void Start()
        {
            storeName = this.gameObject.name;
            myImage = GetComponent<Image>();
            myText = GetComponentInChildren<TextMeshProUGUI>();
            myText.text = storeName;
        }

        public void ReSize(float FOV, float aspect){
            myImage.rectTransform.sizeDelta = new Vector2(FOV * aspect, FOV * aspect);
            myText.fontSize = FOV / 10;
        }
    }


}

