using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ToonJido.Search;
using ToonJido.Data.Model;
using System;

namespace ToonJido.UI{
    public class ButtonMaker : MonoBehaviour
    {
        public GameObject buildingParant;
        public List<BuildingInfo> buildingInfos = new();
        public GameObject buttonPref;
        public GameObject buttonParant;
        void Start()
        {
            buildingParant.GetComponentsInChildren<BuildingInfo>(buildingInfos);
            StartCoroutine(Init());
        }

        IEnumerator Init() {
            while(!SearchManager.instance.isDone){
                yield return null;
            }

            // 중복 제거
            List<BuildingInfo> tempinfo = new();
            tempinfo = buildingInfos.GroupBy(x => x.address).Select(x => x.First()).ToList();

            foreach(var item in tempinfo){
                List<Culture> temp = new();
                temp = SearchManager.instance.storeData.cultures
                    .Where(x => x.find_number == item.address)
                    .ToList();

                var j = 0;
                var k = 0;
                for(int i = 0; i < temp.Count; i++){
                    k++;
                    if(i % 3 == 0){
                        k = 0;
                        j += 1;
                    }
                    GameObject tempObj = Instantiate(buttonPref, 
                                                    item.transform.position +  
                                                    new Vector3(-6f * k, 0f, -6f * j), 
                                                    Quaternion.Euler(new Vector3(90,0,0)), 
                                                    buttonParant.transform);
                    tempObj.name = temp[i].market_name;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

