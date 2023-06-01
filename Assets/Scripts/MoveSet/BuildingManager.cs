using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ToonJido.UI
{
    public class BuildingManager : MonoBehaviour
    {
        public GameObject buildingParant;
        public List<BuildingInfo> buildingInfos;
        public static BuildingManager buildingManager;

        void Awake()
        {
            if (buildingManager == null)
            {
                buildingManager = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            buildingInfos = buildingParant.GetComponentsInChildren<BuildingInfo>().ToList();
        }

        public Vector3 GetBuildingPosition(string address)
        {
            Vector3 test = (
                from item in buildingInfos
                where item.address == address
                select item.navPosition
            ).First();

            return test;
        }
    }
}
