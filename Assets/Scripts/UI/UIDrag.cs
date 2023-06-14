using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ToonJido.Control;
using System.Collections;

namespace ToonJido.UI
{
    public class UIDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        private State tempState;
        public Canvas canvas;
        public RectTransform topRect;
        Rect _safeArea;
        Vector2 _minAnchor;
        Vector2 _maxAnchor;
        float _maxSafe;
        float _minSafe;
        float _halfSafe;
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
            _halfSafe = _maxSafe * 0.3f;
            _minSafe *= -0.9f;

            buttons = sideButtonParant.GetComponentsInChildren<Image>();

            FirstDown();
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
            CurrentControl.state = State.SearchResult;
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
            //transform.position = canvas.transform.TransformPoint(new Vector2(0, _minSafe));
            StartCoroutine(MoveSmooth(transform, canvas.transform.TransformPoint(new Vector2(0, _minSafe)), 0.2f));
            sideButtonParant.SetActive(true);
        }

        public void Half(){
            //transform.position = canvas.transform.TransformPoint(new Vector2(0, _halfSafe));
            StartCoroutine(MoveSmooth(transform, canvas.transform.TransformPoint(new Vector2(0, _halfSafe)), 0.1f));
            sideButtonParant.SetActive(false);
        }

        public void FirstDown(){
            transform.position = canvas.transform.TransformPoint(new Vector2(0, _minSafe));
        }

        public void Up()
        {
            //transform.position = canvas.transform.TransformPoint(new Vector2(0, _maxSafe));
            StartCoroutine(MoveSmooth(transform, canvas.transform.TransformPoint(new Vector2(0, _maxSafe)), 0.2f));
            sideButtonParant.SetActive(false);
            // CurrentControl.ChangeToSearchResult();
        }

        IEnumerator MoveSmooth(Transform origin, Vector3 target, float duringTime){
            float t = 0.0f;
            Vector3 startPosition = origin.position;
            while(t < 1){
                t += Time.deltaTime / duringTime;
                origin.position = Vector3.Lerp(startPosition, target, t);
                yield return null;
            }
        }
    }
}
