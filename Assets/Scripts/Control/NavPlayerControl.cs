using UnityEngine;
using UnityEngine.AI;
using ToonJido.UI;

namespace ToonJido.Control
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavPlayerControl : MonoBehaviour
    {
        private NavMeshAgent myNavMeshAgent;
        public LineRenderer lineRenderer;

        [SerializeField]
        private GameObject clickMarker;

        [SerializeField]
        private Transform visualObjectsParent;
        public static NavPlayerControl navPlayerControl;
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
            lineRenderer.material.mainTextureScale = new Vector2(0.5f, 1f);

            lineRenderer.startWidth = 2f;
            lineRenderer.endWidth = 2f;
            lineRenderer.positionCount = 0;

            noticeManager = NoticeManager.GetInstance();

            //SetDestination(new Vector3(165.33f, 0.0f, 24.1f));
        }

        // Update is called once per frame
        void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     ClickToMove();
            // }

            if (myNavMeshAgent.hasPath)
            {
                DrwaPath();
                if (myNavMeshAgent.remainingDistance < 10f)
                {
                    // 도착 알림
                    noticeManager.ShowNoticeDefaultStyle("목적지 근처에 도착하였습니다.");

                    // 경로 지우기
                    ClearPath();
                }
            }
            
            float offsetX = Time.time - (int)Time.time;
            lineRenderer.material.mainTextureOffset = new Vector2(1 - offsetX, 1);
        }

        private void ClickToMove()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                SetDestination(hit.point);
            }
        }

        public void SetDestination(Vector3 target)
        {
            clickMarker.SetActive(true);
            clickMarker.transform.SetParent(visualObjectsParent);
            myNavMeshAgent.enabled = true;
            if(myNavMeshAgent.isOnNavMesh){
                myNavMeshAgent.SetDestination(target);
                clickMarker.SetActive(true);
                clickMarker.transform.position = target;
            }
            else{
                // 유효하지 않은 위치
                noticeManager.ShowNoticeDefaultStyle("네비게이션을 사용할 수 없는 위치입니다.");
            }
        }

        private void DrwaPath()
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

        private void ClearPath()
        {
            clickMarker.SetActive(false);
            myNavMeshAgent.path.ClearCorners();
            myNavMeshAgent.ResetPath();
            lineRenderer.positionCount = 0;
            myNavMeshAgent.enabled = false;
        }
    }
}
