using System.Collections.Generic;
using System.Linq;
using ToonJido.Control;
using UnityEngine;

public class WalkableSpaceFinder : MonoBehaviour
{
    public GameObject actualGPSCoor;
    public GameObject modifiedGPSCoor;
    public GameObject currGPSCoor;

    bool triggerChecker = false;

    public GPSManager gPSManager;

    Vector3[] findDirs = new Vector3[16]{
        new Vector3(0.0f,   0.0f,   1.0f),
        new Vector3(0.25f,  0.0f,   0.75f),
        new Vector3(0.5f,   0.0f,   0.5f),
        new Vector3(0.75f,  0.0f,   0.25f),
        new Vector3(1.0f,   0.0f,   0.0f),
        new Vector3(0.75f,  0.0f,   -0.25f),
        new Vector3(0.5f,   0.0f,   -0.5f),
        new Vector3(0.25f,  0.0f,   -0.75f),
        new Vector3(0.0f,   0.0f,   -1.0f),
        new Vector3(-0.25f, 0.0f,   -0.75f),
        new Vector3(-0.5f,  0.0f,   -0.5f),
        new Vector3(-0.75f, 0.0f,   -0.25f),
        new Vector3(-1.0f,  0.0f,   0.0f),
        new Vector3(-0.75f, 0.0f,   0.25f),
        new Vector3(-0.5f,  0.0f,   0.5f),
        new Vector3(-0.25f, 0.0f,   0.75f),
    };

    Ray[] rays = new Ray[16]{
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray(),
        new Ray()
    };

    RaycastHit[] raycastHits = new RaycastHit[16]{
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
        new RaycastHit(),
    };

    float[] distances = new float[16];
    
    void Start() {

    }

    void Update() {
        // 트리거가 발생했을 경우엔 OnTrigger의 ShootRay에서 위치 계산
        // 아닌 경우엔 그냥 GPSManager로 뭐시기
        // print($"check is: {triggerChecker}");
        currGPSCoor.transform.position = triggerChecker ? modifiedGPSCoor.transform.position : actualGPSCoor.transform.position;
    }

    private void OnTriggerStay(Collider other) {
        if(other.CompareTag("CanNotStand")){
            triggerChecker = true;
            ShootRay();
        }
    }

    private void OnTriggerExit(Collider other) {
        triggerChecker = false;
    }

    void ShootRay(){
        for(int i = 0; i<rays.Length; i++){
            rays[i].origin = transform.position;
            rays[i].direction = findDirs[i];
        }

        for(int i = 0; i<rays.Length; i++){
            Physics.Raycast(rays[i], out raycastHits[i], 50f, 4096);
            var currDist = transform.position - raycastHits[i].point;
            distances[i] = currDist.magnitude;
#if DEVELOPMENT
            Debug.DrawRay(transform.position, findDirs[i] * 30f, Color.red);
#endif
        }

        // 거리 비교
        bool doWeFound = false;
        int minIndex = 0;
        float minValue = distances[0];
        for(int i = 0; i < distances.Length; i++){
            if(raycastHits[i].point == Vector3.zero)
            {
                
            }
            else
            {
                doWeFound = true;
                if(distances[i] < minValue)
                {
                    minValue = distances[i];
                    minIndex = i;
                }
            }
        }

        print($"Do we Found? {doWeFound}");
        if(doWeFound)
        {
            // print($"index: {minIndex}, dist: {minValue}, pos: {raycastHits[minIndex].point}");
            modifiedGPSCoor.transform.position = raycastHits[minIndex].point;
        }

        doWeFound = false;
        // var temp = distances.Select((item, index) => new{index, subject = item});
        // var nearest = temp.Select(p => p.index).Min();
        // var nearestdist = distances.Min();
        // print($"nearest Dist: {nearestdist}");
        // print($"nearest index: {nearest}, dist: {nearestdist}, Pos: {raycastHits[nearest].point}");
        // currGPSCoor.transform.position = raycastHits[nearest].point;
        //print("index: " + nearest + "position: " + raycastHits[nearest])
    }
}
