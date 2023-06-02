using UnityEngine;
using UnityEngine.AI;

namespace ToonJido.Control
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(LineRenderer))]
    public class NavPlayerControl : MonoBehaviour
    {
        private NavMeshAgent myNavMeshAgent;
        private LineRenderer lineRenderer;

        [SerializeField]
        private GameObject clickMarker;

        [SerializeField]
        private Transform visualObjectsParent;
        public static NavPlayerControl navPlayerControl;

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
            lineRenderer = GetComponent<LineRenderer>();

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
                if (myNavMeshAgent.remainingDistance < 3f)
                {
                    ClearPath();
                }
            }
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
            clickMarker.transform.SetParent(visualObjectsParent);
            myNavMeshAgent.SetDestination(target);
            clickMarker.SetActive(true);
            clickMarker.transform.position = target;
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
            print("clear!");
            myNavMeshAgent.path.ClearCorners();
            // myNavMeshAgent.isStopped = true;
            myNavMeshAgent.ResetPath();
            lineRenderer.positionCount = 0;
        }
    }
}
