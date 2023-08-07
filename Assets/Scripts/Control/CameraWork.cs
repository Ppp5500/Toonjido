using System;
using System.Collections;
using System.Collections.Generic;

using Cinemachine;

using static System.Math;

using ToonJido.Search;
using ToonJido.UI;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR

#elif UNITY_ANDROID
using UnityEngine.Android;

#elif UINTY_IOS
using UnityEngine.iOS;

#endif

namespace ToonJido.Control
{
    public class CameraWork : MonoBehaviour
    {
        [Tooltip("Object to follow")]
        [SerializeField] private GameObject camTarget;
        private Camera mainCamera;
        [SerializeField] Camera UICam;
        [SerializeField] Camera overlookCam;


        [Header("Virtual Cameras")]
        [SerializeField]
        private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();


        [Tooltip("Current use camera")]
        public CinemachineVirtualCamera ActiveCamera = null;
        // virtual 카메라 전환 시 변경할 카메리의 culling mask layer
        int layerDefault;
        int layerPath;
        bool isGPSTracking = false;

        // 부감 카메라 이동관련 변수
        private float spanSpeed = 2f;
        private const float zoomSpeed = 10f;
        private const float twoFingerZoomPower = 3f;
        private const float minFOV = 10, maxFOV = 90, defaultFOV = 50;
        private const float minXPos = -90, maxXPos = 275, minYPos = -360, maxYPos = 180;
        private Vector2 nowPos, prePos;
        private Vector3 movePos;
        private Vector2 nowPos02, prePos02;
        private Vector3 movePos02;
        private Transform PlayerGPSLocation;
        private bool startOnUI01 = true;

        // 아이레벨 카메라 이동관련 변수
        private readonly float rotateSpeed = 2f;
        public float moveSpeed = 30f;
        private GameObject player;
        private Transform eyeLevelTransform;

        // 레이
        private RaycastHit hit;

        //private int layerMask;
        public LayerMask layerMask;
        private float maxRayDis = 50f;
        private GameObject lastEncounter;
        private bool isAleadySearched = false;
        
        // 모델링들
        [Header("Modelings")]
        [SerializeField] GameObject overlookCity;
        [SerializeField] GameObject eyelevelCity;
        [SerializeField] GameObject eyelevelCityExtra;
        [SerializeField] GameObject eyelevelCityExtra02;


#if DEVELOPMENT
        // 테스트용 변수들
        private string debugText;
#endif

        // UI들
        [Header("UI Objects")]
        [SerializeField] private Slider holdSlider;
        [Range(0.3f, 2.0f)]
        public float sliderMaxValue = 0.5f;
        [SerializeField] private GameObject mainUICanvas;
        [SerializeField] private GameObject joyStick;
        public GameObject bottomBar;
        [SerializeField] private GameObject category;
        [SerializeField] private GameObject sideButton;
        [SerializeField] private Toggle gpsToggle;
        [SerializeField] private Button gpsButton;
        [SerializeField] private GameObject zoomInButton;
        [SerializeField] private GameObject zoomOutButton;
        [SerializeField] private Button changeViewButton;
        [SerializeField] private Button weatherButton;
        [SerializeField] private Button weatherBackButton;
        [SerializeField] private CanvasListItem weatherCanvas;
        [SerializeField] private GameObject numberCanvas;
        [SerializeField] private GameObject overCanvas;
        [SerializeField] private CanvasListItem settingCanvas;
        [SerializeField] CanvasListItem menuCanvas;
        [SerializeField] private Button settingBackButton;
        [SerializeField] private Slider moveSpeedSlider;
        [SerializeField] List<GameObject> singCanvaslist = new();
        [SerializeField] GameObject BasementCanvas;
        [SerializeField] GameObject busIconCanvas;


        [Header("UI Resources")]
        [SerializeField] private Sprite overlookIcon;
        [SerializeField] private Sprite eyelevelIcon;

        // common managers
        NoticeManager noticeManager;
        BackKeyManager backKeyManager;
        PPVolumeManager pPVolumeManager;
        EventSystem eventSystem;

        // test
        public Image image;
        public TextMeshProUGUI myText;

        void Start()
        {
            eventSystem = EventSystem.current;
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            eyeLevelTransform = GameObject.Find("EyeLevelCamPos").GetComponent<Transform>();
            PlayerGPSLocation = GameObject.Find("PlayerGPSLocation").GetComponent<Transform>();
            pPVolumeManager = GameObject.Find("PPVolumeManager").GetComponent<PPVolumeManager>();
            player = GameObject.Find("EyelevelPlayer");
            holdSlider.maxValue = sliderMaxValue;

            layerMask = 1 << LayerMask.NameToLayer("Store");
            RenderSettings.fog = false;

            CurrentControl.overlookAction += SwitchCont;
            CurrentControl.eyelevelAction += SwitchCont;
            CurrentControl.searchResultAction += SwitchCont;
            CurrentControl.profileAction += SwitchCont;
            CurrentControl.weatherAction += SwitchCont;

            layerDefault = LayerMask.NameToLayer("Default");
            layerPath = LayerMask.NameToLayer("Path");
            noticeManager = NoticeManager.GetInstance();
            backKeyManager = BackKeyManager.GetInstance();

            gpsButton.onClick.AddListener(() => ResetPosToPlayer());
            changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToEyelevel());
            weatherButton.onClick.AddListener(() => CurrentControl.ChangeToWeather());
            weatherBackButton.onClick.AddListener(() => CurrentControl.ChangeToLastState());
            gpsToggle.onValueChanged.AddListener((value) => GPSFollowOnOff(value));
        }

#if DEVELOPMENT
        private void OnGUI() { }
#endif
        async void Update()
        {
            // UI overray camera와 maincamera의 시야각을 동일하게 맞춰줌
            UICam.fieldOfView = cameras[0].m_Lens.FieldOfView;
            overlookCam.fieldOfView = cameras[0].m_Lens.FieldOfView;

            // float curFOV = cameras[0].m_Lens.FieldOfView;
            // switch (curFOV)
            // {
            //     case float value when value <= 30:
            //         SwitchSign(singCanvaslist[0]);
            //         break;

            //     case float value when 40 <= value && value <= 50:
            //         SwitchSign(singCanvaslist[1]);
            //         break;

            //     case float value when 50 < value:
            //         SwitchSign(singCanvaslist[2]);
            //         break;
            // }
            // image.rectTransform.sizeDelta = new Vector2(curFOV * 0.3f, curFOV * 0.3f);
            // myText.fontSize = curFOV/10;

            // 현재 조작 모드가 부감일 때
            if (CurrentControl.state == State.Overlook)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    if(eventSystem.IsPointerOverGameObject(touch.fingerId)){
                        return;
                    }

                    if (touch.phase == TouchPhase.Began)
                    {
                        movePos = Vector3.zero;

                        prePos = touch.position - touch.deltaPosition;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 화면 이동 속도 업데이트
                        spanSpeed = CalSpanSpeed(mainCamera.fieldOfView);

                        // 실제 카메라 이동 로직
                        nowPos = touch.position - touch.deltaPosition;
                        movePos = (Vector3)(prePos - nowPos) * spanSpeed * Time.deltaTime;

                        // CamTarget의 위치 제한
                        if(minXPos < camTarget.transform.position.x + movePos.x && camTarget.transform.position.x + movePos.x < maxXPos){
                            if(minYPos < camTarget.transform.position.z + movePos.y && camTarget.transform.position.z + movePos.y < maxYPos)
                            camTarget.transform.Translate(movePos);
                        }
                        
                        prePos = touch.position - touch.deltaPosition;
                    }

                    // 두 손가락 컨트롤 로직
                    if (Input.touchCount == 2)
                    {
                        Touch touch02 = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touch.position - touch.deltaPosition;
                        Vector2 touchOnePrevPos = touch02.position - touch02.deltaPosition;

                        if (touch02.phase == TouchPhase.Moved)
                        {
                            float prevTouchDeltaMag = (
                                touchZeroPrevPos - touchOnePrevPos
                            ).magnitude;
                            float touchDeltaMag = (touch.position - touch02.position).magnitude;

                            float deltaDiff = prevTouchDeltaMag - touchDeltaMag;
                            var newFOV = cameras[0].m_Lens.FieldOfView +=
                                twoFingerZoomPower * deltaDiff * Time.deltaTime;
                            cameras[0].m_Lens.FieldOfView = Clamp(newFOV, minFOV, maxFOV);
                        }
                    }
                }
            }

            else if (CurrentControl.state == State.Eyelevel)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    // if(eventSystem.IsPointerOverGameObject(touch.fingerId)){
                    //     return;
                    // }

                    if (touch.phase == TouchPhase.Began)
                    {
                        // 터치가 UI위에서 시작되었는지 검사
                        startOnUI01 = eventSystem.IsPointerOverGameObject(touch.fingerId);

                        // 터치가 UI위에서 시작되지 않았다면
                        // 최초 터치 위치 계산
                        if(startOnUI01 is false){
                            prePos = touch.position - touch.deltaPosition;
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 터치가 UI위에서 시작되지 않았다면
                        // 카메라 회전
                        if(startOnUI01 is false){
                            // 카메라 회전 로직
                            nowPos = touch.position - touch.deltaPosition;
                            movePos = (Vector3)(prePos - nowPos) * rotateSpeed * Time.deltaTime;

                            var verAnglePow = movePos.y * -1;
                            var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                            if (nextVerAngle > 180)
                                nextVerAngle -= 360;

                            if (-30 < nextVerAngle && nextVerAngle < 30)
                            {
                                eyeLevelTransform.rotation *= Quaternion.AngleAxis(
                                    verAnglePow,
                                    Vector3.right
                                );
                            }

                            player.transform.Rotate(new Vector3(0, movePos.x, 0), Space.Self);
                            prePos = touch.position - touch.deltaPosition;
                        }
                        holdSlider.value =0;
                    }
                    else if(touch.phase == TouchPhase.Stationary){
                        if(startOnUI01 is false){
                            GetEncounter(ref lastEncounter);
                            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

                            if (Physics.Raycast(ray, out hit, maxRayDis, layerMask) && !isAleadySearched)
                            {
                                holdSlider.gameObject.SetActive(true);
                                holdSlider.gameObject.transform.position = touch.position + new Vector2(0f, 80f);
                                holdSlider.value += Time.deltaTime;
                                if(holdSlider.value > sliderMaxValue - 0.01f){
                                    isAleadySearched = true;
                                    holdSlider.gameObject.SetActive(false);
                                    var address = lastEncounter.GetComponent<BuildingInfo>().address;
                                    var result = SearchManager.instance.SearchStoreByAddress(address);
                                    await SearchManager.instance.DisplayResult(result);
                                }
                            }
                        }

                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        lastEncounter = null;
                        isAleadySearched = false;
                        holdSlider.gameObject.SetActive(false);
                        holdSlider.value =0;
                    }

                    // 두 손가락 컨트롤 로직
                    if (Input.touchCount == 2)
                    {
                        Touch touch02 = Input.GetTouch(1);

                        if (touch02.phase == TouchPhase.Began)
                        {
                            prePos02 = touch02.position - touch02.deltaPosition;
                        }
                        else if (touch02.phase == TouchPhase.Moved)
                        {
                            // 카메라 회전 로직
                            nowPos02 = touch02.position - touch02.deltaPosition;
                            movePos02 =
                                (Vector3)(prePos02 - nowPos02) * rotateSpeed * Time.deltaTime;

                            var verAnglePow = movePos02.y * -1;
                            var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                            if (nextVerAngle > 180)
                                nextVerAngle -= 360;

                            if (-30 < nextVerAngle && nextVerAngle < 30)
                            {
                                eyeLevelTransform.rotation *= Quaternion.AngleAxis(
                                    verAnglePow,
                                    Vector3.right
                                );
                            }

                            player.transform.Rotate(new Vector3(0, movePos02.x, 0), Space.Self);
                            prePos02 = touch02.position - touch02.deltaPosition;
                        }
                    }
                }
            }

            // 현재 조작 모드가 날씨일 때
            if (CurrentControl.state == State.Weather)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        prePos = touch.position - touch.deltaPosition;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // 카메라 회전 로직
                        nowPos = touch.position - touch.deltaPosition;
                        movePos = (Vector3)(prePos - nowPos) * rotateSpeed * Time.deltaTime;

                        var verAnglePow = movePos.y * -1;
                        var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                        if (nextVerAngle > 180)
                            nextVerAngle -= 360;

                        if (-30 < nextVerAngle && nextVerAngle < 30)
                        {
                            eyeLevelTransform.rotation *= Quaternion.AngleAxis(
                                verAnglePow,
                                Vector3.right
                            );
                        }

                        player.transform.Rotate(new Vector3(0, movePos.x, 0), Space.Self);
                        prePos = touch.position - touch.deltaPosition;
                    }
                }
            }
        }

        void FixedUpdate() {
            if(isGPSTracking){
                camTarget.transform.position = PlayerGPSLocation.position;
            }
        }

        void GPSFollowOnOff(bool input){
            if(input){
                if(CurrentControl.gpsStatus == GPSStatus.avaliable){
                    isGPSTracking = true;
                }
                else{
                    noticeManager.ShowNoticeDefaultStyle("GPS 사용 불가 시에는 따라가기 기능을 이용할 수 없습니다.");
                    gpsToggle.isOn = false;
                }
            }
            else{
                isGPSTracking = false;
            }
            
        }
        
        // 카메라 줌인
        public void CamZoomIn()
        {
            var tempFOV = Clamp(cameras[0].m_Lens.FieldOfView - 10, minFOV, maxFOV);
            var moveFOV = Abs(cameras[0].m_Lens.FieldOfView) - Abs(tempFOV);
            StartCoroutine(CamMove((int)moveFOV));
        }

        // 카메라 줌아웃
        public void CamZoomOut()
        {
            var tempFOV = Clamp(cameras[0].m_Lens.FieldOfView + 10, minFOV, maxFOV);
            var moveFOV = Abs(cameras[0].m_Lens.FieldOfView) - Abs(tempFOV);
            StartCoroutine(CamMove((int)moveFOV));
        }

        // 카메라 줌 코루틴
        public IEnumerator CamMove(int input)
        {
            if (input < 0)
            {
                input = Abs(input);
                for (int i = 0; i < input; i++)
                {
                    cameras[0].m_Lens.FieldOfView += 1;
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                for (int i = 0; i < input; i++)
                {
                    cameras[0].m_Lens.FieldOfView -= 1;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        private float CalSpanSpeed(float input)
        {
            return input / 10;
        }

        /// <summary>
        /// 컨트롤 상태 변화 시에 호출될 메소드
        /// </summary>
        public void SwitchCont()
        {
            if (CurrentControl.state == State.Overlook)
            {
                SwitchCamera(cameras[0]);

                mainCamera.cullingMask = (1 << layerDefault);
                UICam.gameObject.SetActive(true);
                overlookCam.gameObject.SetActive(true);

                mainUICanvas.SetActive(true);
                joyStick.SetActive(false);
                bottomBar.SetActive(true);
                category.SetActive(true);
                sideButton.SetActive(true);
                gpsToggle.gameObject.SetActive(true);
                gpsButton.gameObject.SetActive(false);
                zoomInButton.SetActive(true);
                zoomOutButton.SetActive(true);
                BasementCanvas.SetActive(true);
                busIconCanvas.SetActive(true);

                changeViewButton.onClick.RemoveAllListeners();
                changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToEyelevel());
                overlookCity.SetActive(true);
                eyelevelCity.SetActive(false);
                eyelevelCityExtra.SetActive(false);
                eyelevelCityExtra02.SetActive(false);

                RenderSettings.fog = false;
                pPVolumeManager.TurnOffDepthOfField();
                if(weatherCanvas.gameObject.activeSelf){weatherCanvas.SetActive(false);}
                changeViewButton.image.sprite = eyelevelIcon;
            }
            else if (CurrentControl.state == State.Eyelevel)
            {
                SwitchCamera(cameras[1]);
                
                mainCamera.cullingMask = (1 << layerDefault) | (1 << layerPath);

                UICam.gameObject.SetActive(false);
                overlookCam.gameObject.SetActive(false);

                holdSlider.gameObject.SetActive(true);
                mainUICanvas.SetActive(true);
                joyStick.SetActive(true);
                bottomBar.SetActive(false);
                category.SetActive(false);
                sideButton.SetActive(true);
                gpsToggle.gameObject.SetActive(false);
                gpsButton.gameObject.SetActive(true);
                zoomInButton.SetActive(false);
                zoomOutButton.SetActive(false);
                BasementCanvas.SetActive(false);
                busIconCanvas.SetActive(false);

                changeViewButton.onClick.RemoveAllListeners();
                changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToOverlook());
                overlookCity.SetActive(false);
                eyelevelCity.SetActive(true);
                eyelevelCityExtra.SetActive(true);
                eyelevelCityExtra02.SetActive(true);


                StartCoroutine(
                    WaitThenCallback( 0.7f,() =>
                        {
                            if (CurrentControl.state == State.Eyelevel)
                            {
                                RenderSettings.fog = true;
                                RenderSettings.fogDensity = 0.005f;
                            }
                        }
                    )
                );
                pPVolumeManager.TurnOnDepthOfField();
                if(weatherCanvas.gameObject.activeSelf){weatherCanvas.SetActive(false);}
                changeViewButton.image.sprite = overlookIcon;
            }
            else if (CurrentControl.state == State.SearchResult)
            {
                joyStick.SetActive(false);
                sideButton.SetActive(false);
            }
            else if (CurrentControl.state == State.Profile)
            {
                joyStick.SetActive(false);
                sideButton.SetActive(false);
            }
            else if (CurrentControl.state == State.Weather)
            {
                if(CurrentControl.lastState == State.Overlook){
                    SwitchCamera(cameras[2]);
                }

                mainCamera.cullingMask = (1 << layerDefault) | (1 << layerPath);
                UICam.gameObject.SetActive(false);
                overlookCam.gameObject.SetActive(false);
                
                holdSlider.value = 0f;
                holdSlider.gameObject.SetActive(false);
                mainUICanvas.SetActive(false);
                joyStick.SetActive(false);
                sideButton.SetActive(false);
                if(menuCanvas.gameObject.activeSelf) { menuCanvas.SetActive(false); }
                weatherCanvas.SetActive(true);
                BasementCanvas.SetActive(false);

                overlookCity.SetActive(false);
                eyelevelCity.SetActive(true);
                eyelevelCityExtra.SetActive(true);
                eyelevelCityExtra02.SetActive(true);

                StartCoroutine(
                    WaitThenCallback( 0.7f,() =>
                        {
                            if (CurrentControl.state == State.Weather)
                            {
                                RenderSettings.fog = true;
                                RenderSettings.fogDensity = 0.005f;
                            }
                        }
                    )
                );
            }
        }

        private IEnumerator WaitThenCallback(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }

        /// <summary>
        /// 레이에 맞은 오브젝트를 반환, 최대 거리와 레이어마스크는 이미 작성되어 있음
        /// </summary>
        /// <param name="output"></param>
        private bool GetEncounter(ref GameObject output)
        {
            // if(!startOnUI01){
            //     output = null;
            //     return false;
            // }
#if UNITY_EDITOR
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, maxRayDis, layerMask))
            {
                output = hit.collider.gameObject;
                return true;
            }
            else
            {
                output = null;
                return false;
            }
#elif UNITY_ANDROID
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out hit, maxRayDis, layerMask))
            {
                output = hit.collider.gameObject;
                return true;
            }
            else
            {
                output = null;
                return false;
            }
#elif UNITY_IOS
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(ray, out hit, maxRayDis, layerMask))
            {
                output = hit.collider.gameObject;
                return true;
            }
            else
            {
                output = null;
                return false;
            }
#endif
        }

        // overlook cam과 eyelevel cam 사이의 전환
        public void SwitchCamera(CinemachineVirtualCamera cam)
        {
            cam.Priority = 1;
            ActiveCamera = cam;

            foreach (var c in cameras)
            {
                if (c != cam)
                    c.Priority = 0;
            }
        }

        private void SwitchSign(GameObject _signCanvas)
        {
            _signCanvas.SetActive(true);

            foreach(var item in singCanvaslist)
            {
                if(item != _signCanvas)
                {
                    item.SetActive(false);
                }
            }
        }

        // 현재 사용자 위치로 카메라 이동
        public void ResetPosToPlayer()
        {
            if(CurrentControl.gpsStatus is not GPSStatus.avaliable){
                noticeManager.ShowNoticeDefaultStyle("GPS 사용 불가 시에는 현재 위치 이동 기능을 이용할 수 없습니다.");
                return;
            }

            if (CurrentControl.state == State.Overlook)
            {
                camTarget.transform.position = PlayerGPSLocation.transform.position;
            }
            else if (CurrentControl.state == State.Eyelevel)
            {
                player.transform.position = PlayerGPSLocation.transform.position;
            }
        }
    }
}




                

            // 현재 조작 모드가 아이레벨일 때
            // else if (CurrentControl.state == State.Eyelevel)
            // {
            //     if (Input.touchCount > 0)
            //     {
            //         Touch touch = Input.GetTouch(0);

            //         if (touch.phase == TouchPhase.Began)
            //         {
            //             startOnUI01 = IsPointerOverUI(touch.fingerId) ? true : false;
            //             // 터치가 UI위에서 시작된 경우 카메라 이동 false
            //             if (startOnUI01)
            //             {
            //                 startOnBuilding01 = false;
            //                 return;
            //             }
            //             else
            //             {
            //                 // player.transform.Translate(
            //                 //     moveInput * moveSpeed * Time.deltaTime,
            //                 //     Space.Self
            //                 // );
            //                 prePos = touch.position - touch.deltaPosition;

            //                 startOnBuilding01 = true;
            //                 // 첫 터치 때 레이에 맞은 오브젝트 감지
            //                 if (
            //                     GetEncounter(
            //                         ref firstEncounter,
            //                         out var position,
            //                         out var norVec,
            //                         startOnBuilding01
            //                     )
            //                 )
            //                 {
            //                     HitPos.transform.position = position;
            //                     HitPos.transform.up = norVec;
            //                     // 터치 시작을 건물에서 시작하면 시점 변환 X
            //                     startOnBuilding01 = true;
            //                     startOnUI01 = true;
            //                 }
            //                 else
            //                 {
            //                     startOnBuilding01 = false;
            //                 }
            //             }
            //         }
            //         else if (touch.phase == TouchPhase.Moved)
            //         {
            //             if (startOnBuilding01)
            //             {
            //                 // 터치 이동 시 레이에 맞은 오브젝트 감지
            //                 if (
            //                     GetEncounter(
            //                         ref lastEncounter,
            //                         out var position,
            //                         out var norVec,
            //                         startOnBuilding01
            //                     )
            //                 )
            //                 {
            //                     HitPos.transform.position = position;
            //                     HitPos.transform.up = norVec;
            //                 }
            //                 else
            //                 {
            //                     startOnBuilding01 = false;
            //                     HitPos.transform.position = new(0, -1, 0);
            //                     firstEncounter = null;
            //                     lastEncounter = null;
            //                 }
            //             }

            //             if (startOnUI01)
            //             {
            //                 return;
            //             }
            //             else
            //             {
            //                 // 카메라 회전 로직
            //                 nowPos = touch.position - touch.deltaPosition;
            //                 movePos = (Vector3)(prePos - nowPos) * rotateSpeed * Time.deltaTime;

            //                 var verAnglePow = movePos.y * -1;
            //                 var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
            //                 if (nextVerAngle > 180)
            //                     nextVerAngle -= 360;

            //                 if (-30 < nextVerAngle && nextVerAngle < 30)
            //                 {
            //                     eyeLevelTransform.rotation *= Quaternion.AngleAxis(
            //                         verAnglePow,
            //                         Vector3.right
            //                     );
            //                 }

            //                 player.transform.Rotate(new Vector3(0, movePos.x, 0), Space.Self);
            //                 prePos = touch.position - touch.deltaPosition;
            //             }
            //         }
            //         else if(touch.phase == TouchPhase.Stationary){
            //             if(startOnBuilding01){
            //                 GetEncounter(ref lastEncounter);
            //             }
            //         }
            //         else if (touch.phase == TouchPhase.Ended)
            //         {
            //             if (startOnBuilding01)
            //             {
            //                 // 터치를 끝낼 때의 오브젝트와 터치를 시작할 때의 오브젝트가 같은지 검사
            //                 if (CheckSameObject(firstEncounter, lastEncounter))
            //                 {
            //                     var address = lastEncounter.GetComponent<BuildingInfo>().address;
            //                     var result = SearchManager.instance.SearchStoreByAddress(address);
            //                     SearchManager.instance.DisplayResult(result);
            //                 }
            //                 HitPos.transform.position = new(0, -2, 0);
            //                 firstEncounter = null;
            //                 lastEncounter = null;
            //                 startOnBuilding01 = false;
            //             }
            //         }

                    // // 두 손가락 컨트롤 로직
                    // if (Input.touchCount == 2)
                    // {
                    //     Touch touch02 = Input.GetTouch(1);

                    //     if (touch02.phase == TouchPhase.Began)
                    //     {
                    //         startOnUI02 = IsPointerOverUI(touch02.fingerId) ? true : false;
                    //         // 터치가 UI위에서 시작된 경우 카메라 이동 false
                    //         if (startOnUI02)
                    //         {
                    //             startOnBuilding02 = false;
                    //             return;
                    //         }
                    //         else
                    //         {
                    //             player.transform.Translate(
                    //                 moveInput * moveSpeed * Time.deltaTime,
                    //                 Space.Self
                    //             );

                    //             prePos02 = touch02.position - touch02.deltaPosition;
                    //         }
                    //     }
                    //     else if (touch02.phase == TouchPhase.Moved)
                    //     {
                    //         if (startOnUI02)
                    //         {
                    //             return;
                    //         }
                    //         else
                    //         {
                    //             // 카메라 회전 로직
                    //             nowPos02 = touch02.position - touch02.deltaPosition;
                    //             movePos02 =
                    //                 (Vector3)(prePos02 - nowPos02) * rotateSpeed * Time.deltaTime;

                    //             var verAnglePow = movePos02.y * -1;
                    //             var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                    //             if (nextVerAngle > 180)
                    //                 nextVerAngle -= 360;

                    //             if (-30 < nextVerAngle && nextVerAngle < 30)
                    //             {
                    //                 eyeLevelTransform.rotation *= Quaternion.AngleAxis(
                    //                     verAnglePow,
                    //                     Vector3.right
                    //                 );
                    //             }

                    //             player.transform.Rotate(new Vector3(0, movePos02.x, 0), Space.Self);
                    //             prePos02 = touch02.position - touch02.deltaPosition;
                    //         }
                    //     }
                    // }
