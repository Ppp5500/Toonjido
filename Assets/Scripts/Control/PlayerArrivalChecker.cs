using System.Collections;
using System.Collections.Generic;
using ToonJido.UI;
using UnityEngine;

namespace ToonJido.Control{
    public class PlayerArrivalChecker : MonoBehaviour
    {
        [SerializeField] private PathDrawer pathDrawer;
        private bool isPathFindingOn = false;
        private Vector3 destination;
        private Transform actualGPSTransform;

        // common managers
        private NoticeManager noticeManager;

        void Start(){
            noticeManager = NoticeManager.GetInstance();
            actualGPSTransform = this.GetComponent<Transform>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isPathFindingOn) CheckArrival();
        }

        public void CheckArrival(){
            float distanceToTarget = Vector3.Distance(actualGPSTransform.position, destination);
            print($"dist: {distanceToTarget}");
            if (distanceToTarget < 20f)
            {
                // 도착 알림
                noticeManager.ShowNoticeDefaultStyle("목적지 근처에 도착하였습니다.");

                // 경로 지우기
                pathDrawer.EraseLine();

                isPathFindingOn = false;
            }
        }

        public void SetArrivalChecker(Vector3 _destination){
            isPathFindingOn = true;
            destination = _destination;
        }
    }
}

