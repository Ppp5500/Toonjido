using Newtonsoft.Json.Linq;
using TMPro;
using ToonJido.Control;
using UnityEngine;
using UnityEngine.UI;

namespace ToonJido.UI{
    [RequireComponent(typeof(LineRenderer))]
    public class PathDrawer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [Header("Line Width")]
        [SerializeField] private float lineStartWidth;
        [SerializeField] private float lineEndWidth;

        [Header("Marker Objects")]
        [SerializeField] private GameObject startMarker;
        [SerializeField] private GameObject endMarker;

        [Header("Stop UI Elements")]
        [SerializeField] private GameObject pathDrawStopButtonParent;
        [SerializeField] private Button pathDrawStopButton;
        [SerializeField] private TextMeshProUGUI destinationText;
        
        // Start is called before the first frame update
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = lineStartWidth;
            lineRenderer.endWidth = lineEndWidth;

            pathDrawStopButton.onClick.AddListener(EraseLine);
        }

        public void DrawLine(JToken[] _path, string _storeName)
        {
            lineRenderer.positionCount = _path.Length;
            
            for(int i = 0; i < _path.Length; i ++)
            {
                var point = GPSEncoder.GPSToUCS((float)_path[i][1], (float)_path[i][0]) + new Vector3(0, 0.05f, 0);
                lineRenderer.SetPosition(i, point);
            }

            startMarker.SetActive(true);
            endMarker.SetActive(true);
            startMarker.transform.position = lineRenderer.GetPosition(0) + Vector3.up;
            endMarker.transform.position = lineRenderer.GetPosition(_path.Length - 1) + Vector3.up;

            pathDrawStopButtonParent.SetActive(true);
            destinationText.text = _storeName;
        }

        public void EraseLine(){
            startMarker.SetActive(false);
            endMarker.SetActive(false);

            lineRenderer.positionCount = 0;
            pathDrawStopButtonParent.SetActive(false);
        }
    }
}

