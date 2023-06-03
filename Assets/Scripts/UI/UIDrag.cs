using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ToonJido.Control;

namespace ToonJido.UI
{
    public class UIDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private CurrentControl.State tempState;
        public Canvas canvas;
        public RectTransform topRect;
        Rect _safeArea;
        Vector2 _minAnchor;
        Vector2 _maxAnchor;
        float _maxSafe;
        float _minSafe;
        float canvasSize;
        float topRectSize;
        public GameObject sideButtonParant;
        private Image[] buttons;

        void Start()
        {
            _safeArea = Screen.safeArea;
            canvasSize = canvas.GetComponent<RectTransform>().sizeDelta.y;
            topRectSize = topRect.sizeDelta.y;

            _minAnchor = _safeArea.position;
            _maxAnchor = _minAnchor + _safeArea.size;
            var temp = _minAnchor.y;

            _minAnchor.x /= Screen.width;
            _minAnchor.y /= Screen.height;
            _maxAnchor.x /= Screen.width;
            _maxAnchor.y /= Screen.height;

            _minSafe = canvasSize / 2;
            _maxSafe = _minSafe - (temp + topRectSize);
            _minSafe *= -0.9f;

            buttons = sideButtonParant.GetComponentsInChildren<Image>();

            Down();
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            PointerEventData data = eventData;

            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)canvas.transform,
                data.position,
                canvas.worldCamera,
                out position
            );
            position.x = 0;
            position.y = Mathf.Clamp(position.y, _minSafe, _maxSafe);
            transform.position = canvas.transform.TransformPoint(position);


        }

        float normalize(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            tempState = CurrentControl.state;
            CurrentControl.state = CurrentControl.State.SearchResult;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            CurrentControl.state = tempState;
            if(transform.position.y > (Screen.height/2)){
                Up();
            }
            else{
                Down();
            }
        }

        public void Down()
        {
            transform.position = canvas.transform.TransformPoint(new Vector2(0, _minSafe));
            sideButtonParant.SetActive(true);
        }

        public void Up()
        {
            transform.position = canvas.transform.TransformPoint(new Vector2(0, _maxSafe));
            sideButtonParant.SetActive(false);

        }
    }
}
