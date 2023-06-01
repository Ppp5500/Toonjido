using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ToonJido.Data.Model;
using ToonJido.UI;
using ToonJido.Control;

public class StoreInfo : MonoBehaviour
{
    public Culture myCulture { get; set; }
    public GameObject parent;
    public Button pathButton;

    // Start is called before the first frame update
    void Start() { 
        // for test
        myCulture = new(){
            id = 01,
            market_name = "조양 식당",
            lot_number = "235"
        };
        print(myCulture.lot_number);

        SetListItem();
    }

    // Update is called once per frame
    void Update() { }

    public void SetListItem()
    {
        pathButton.onClick.AddListener(PathFind);
    }

    public void PathFind()
    {
        Vector3 lot = BuildingManager.buildingManager.GetBuildingPosition(myCulture.lot_number);
        NavPlayerControl.navPlayerControl.SetDestination(lot);
    }
}
