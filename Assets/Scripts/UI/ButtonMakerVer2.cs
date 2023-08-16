using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToonJido.Data.Model;
using ToonJido.Search;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMakerVer2 : MonoBehaviour
{
    public GameObject buildingParant;
    public List<BuildingInfo> buildingInfos = new();
    public GameObject buttonPref;

    public List<GameObject> buttonParents;
    public List<Sprite> buttonSprites;

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
                .Where(x => x.latitude != null)
                .ToList();

            for(int i = 0; i < temp.Count; i++){

                int parentIndex = temp[i].category switch
                {
                    1 => 0,
                    2 => 1,
                    3 => 2,
                    4 => 3,
                    5 => 4,
                    _ => 0
                };

                // 프리팹 생성
                GameObject tempObj = Instantiate(buttonPref, 
                                                GPSEncoder.GPSToUCS((float)temp[i].latitude, (float)temp[i].longitude), 
                                                Quaternion.Euler(new Vector3(90,0,0)), 
                                                buttonParents[parentIndex].transform);
                tempObj.GetComponent<UnityEngine.UI.Image>().sprite = buttonSprites[parentIndex];
                
                tempObj.name = temp[i].market_name;
                tempObj.GetComponent<Button>().onClick
                    .AddListener(()
                    =>{
                        SearchManager.instance.OpenDetailCanvas(tempObj.name);
                    });
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
