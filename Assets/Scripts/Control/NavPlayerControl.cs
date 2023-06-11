using UnityEngine;
using UnityEngine.AI;

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
        public GameObject noticeCanvas;

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


            lineRenderer.startWidth = 4f;
            lineRenderer.endWidth = 4f;
            lineRenderer.positionCount = 0;
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
                if (myNavMeshAgent.remainingDistance < 5f)
                {
                    // 도착 알림
                    
                    // 경로 지우기
                    ClearPath();
                }
            }
            
            // Animates main texture scale in a funky way!
            // float scaleX = Mathf.Cos(Time.time) * 0.5f + 1;
            // float scaleY = Mathf.Sin(Time.time) * 0.5f + 1;
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
                noticeCanvas.SetActive(true);
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
            // myNavMeshAgent.isStopped = true;
            myNavMeshAgent.ResetPath();
            lineRenderer.positionCount = 0;
            myNavMeshAgent.enabled = false;
        }
    }
}
