using UnityEngine;
using UnityEngine.AI;
using ToonJido.UI;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ToonJido.Control
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavPlayerControl : MonoBehaviour
    {
        private NavMeshAgent myNavMeshAgent;
        private Transform mytransform;
        public LineRenderer lineRenderer;
        float t = 0f;

        Coroutine pathRefinder = null;

        [SerializeField] private GameObject clickMarker;
        [SerializeField] GameObject startMarker;
        [SerializeField] private Transform visualObjectsParent;

        [SerializeField] Transform currGPSCoor;
        [SerializeField] GameObject pathFindExitParent;
        [SerializeField] Button pathFindExitButton;
        [SerializeField] TextMeshProUGUI currentPathObject;

        // singleton
        public static NavPlayerControl navPlayerControl;

        // common manager
        private NoticeManager noticeManager;

        void Awake()
        {
            if (navPlayerControl == null)
            {
                navPlayerControl = this;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            myNavMeshAgent = GetComponent<NavMeshAgent>();
            mytransform = GetComponent<Transform>();
            lineRenderer.material.mainTextureScale = new Vector2(0.5f, 1f);

            lineRenderer.startWidth = 2f;
            lineRenderer.endWidth = 2f;
            lineRenderer.positionCount = 0;

            noticeManager = NoticeManager.GetInstance();

            pathFindExitButton.onClick.AddListener(() => ClearPath());

            CurrentControl.eyelevelAction += EyelevelSetting;
            CurrentControl.overlookAction += OverlookSetting;

            //SetDestination(new Vector3(165.33f, 0.0f, 24.1f));
        }

        // Update is called once per frame
        void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     ClickToMove();
            // }

            mytransform.position = currGPSCoor.position;

            if (myNavMeshAgent.hasPath)
            {
                DrawPath();

                float distanceToTarget = Vector3.Distance(mytransform.position, myNavMeshAgent.destination);
                if (distanceToTarget < 20f)
                {
                    print($"am i have path? {myNavMeshAgent.hasPath}");
                    // 도착 알림
                    noticeManager.ShowNoticeDefaultStyle("목적지 근처에 도착하였습니다.");

                    // 경로 지우기
                    ClearPath();
                }
            }
            
            float offsetX = Time.time - (int)Time.time;
            lineRenderer.material.mainTextureOffset = new Vector2(1 - offsetX, 1);
        }

        // private void ClickToMove()
        // {
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     bool hasHit = Physics.Raycast(ray, out hit);
        //     if (hasHit)
        //     {
        //         SetDestination(hit.point);
        //     }
        // }

        public void SetDestination(Vector3 target, string pathObjectName)
        {
            myNavMeshAgent.enabled = true;
            if(myNavMeshAgent.isOnNavMesh){
                myNavMeshAgent.SetDestination(target);
                clickMarker.SetActive(true);
                clickMarker.transform.SetParent(visualObjectsParent);
                clickMarker.transform.position = target;

                startMarker.SetActive(true);
                startMarker.transform.SetParent(visualObjectsParent);
                startMarker.transform.position = currGPSCoor.position;

                // UI 설정
                pathFindExitParent.SetActive(true);
                currentPathObject.text = pathObjectName;

                if(CurrentControl.state == State.Eyelevel){
                    EyelevelSetting();
                }
                else{
                    OverlookSetting();
                }

                // 코루틴 설정
                if(pathRefinder != null){
                    StopCoroutine(pathRefinder);
                }
                pathRefinder = StartCoroutine(ReFind(target));
            }
            else{
                // 유효하지 않은 위치
                noticeManager.ShowNoticeDefaultStyle("네비게이션을 사용할 수 없는 위치입니다.");
            }
        }

        void ReCalPath(Vector3 _target){
            if(myNavMeshAgent.isOnNavMesh){
                //myNavMeshAgent.path.ClearCorners();
                myNavMeshAgent.ResetPath();
                // lineRenderer.positionCount = 0;
                myNavMeshAgent.SetDestination(_target);
            }
            else{
                // 유효하지 않은 위치
                noticeManager.ShowNoticeDefaultStyle("네비게이션을 사용할 수 없는 위치입니다.");
            }
        }

        // 1초에 한번씩 ReCalPath를 호출하는 코루틴
        IEnumerator ReFind(Vector3 _target){
            while(true){
                yield return new WaitForSeconds(1);
                ReCalPath(_target);
            }
        }

        // 경로 그리기
        private void DrawPath()
        {
            lineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
            lineRenderer.SetPosition(0, transform.position);

            if (myNavMeshAgent.path.corners.Length < 2)
            {
                return;
            }

            for (int i = 1; i < myNavMeshAgent.path.corners.Length; i++)
            {
                Vector3 pointPosition = new Vector3(
                    myNavMeshAgent.path.corners[i].x,
                    myNavMeshAgent.path.corners[i].y,
                    myNavMeshAgent.path.corners[i].z
                );
                lineRenderer.SetPosition(i, pointPosition);
            }
        }

        // 경로 삭제
        private void ClearPath()
        {
            print("destination: " + myNavMeshAgent.destination);
            pathFindExitParent.SetActive(false);
            StopCoroutine(pathRefinder);
            clickMarker.SetActive(false);
            startMarker.SetActive(false);
            myNavMeshAgent.path.ClearCorners();
            myNavMeshAgent.ResetPath();
            myNavMeshAgent.isStopped = true;
            lineRenderer.positionCount = 0;
            myNavMeshAgent.enabled = false;
        }

        // 뷰에 따라 linerenderer 설정 변경
        public void OverlookSetting(){
            lineRenderer.startWidth = 5f;
            lineRenderer.endWidth = 5f;
        }

        public void EyelevelSetting(){
            lineRenderer.startWidth = 2f;
            lineRenderer.endWidth = 2f;
        }
    }
}
