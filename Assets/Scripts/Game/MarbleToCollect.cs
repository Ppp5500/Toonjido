using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToonJido.Game{
    public class MarbleToCollect : MonoBehaviour
    {
        public int stars;
        public SevenStarMarbleGameManager marbleGameManager;

        private void Start() {
            marbleGameManager = SevenStarMarbleGameManager.GetInstance();
        }
        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.CompareTag("Player")){
                marbleGameManager.FoundMarble(stars.ToString());
                FortuneWindowManager.GetInstance().ShowFortune();
                Destroy(this.gameObject);
            }
        }
    }
}