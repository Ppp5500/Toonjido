using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Math;
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
        private EventSystem eventSystem;

        [Header("Virtual Cameras")]
        [SerializeField]
        private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();
        [Tooltip("Current use camera")]
        public CinemachineVirtualCamera ActiveCamera = null;

        // �ΰ� ī�޶� �̵����� ����
        private float spanSpeed = 2f;
        private float zoomSpeed = 10f;
        private float twoFingerZoomPower = 3f;
        private float minFOV = 20, maxFOV = 70, defaultFOV = 50;
        private Vector2 nowPos, prePos;
        private Vector3 movePos;
        private Vector2 nowPos02, prePos02;
        private Vector3 movePos02;
        private Transform PlayerGPSLocation;
        private Transform overlookTransform;
        private bool startOnUI01 = true;
        private bool startOnUI02 = true;


        // ���̷��� ī�޶� �̵����� ����
        private float rotateSpeed = 2f;
        private float moveSpeed = 30f;
        private GameObject player;
        private Transform eyeLevelTransform;
        private Vector3 moveInput;

        // ����
        private RaycastHit hit;
        private int layerMask;
        private float maxRayDis = 50f;
        private GameObject firstEncounter;
        private GameObject lastEncounter;

        // �׽�Ʈ�� ������
        private string rayObj;
        private string debugText;


        // UI��
        [Header("UI Objects")]
        [SerializeField] private GameObject joyStick;
        [SerializeField] private GameObject sideButton;
        [SerializeField] private Button resetPosButton;
        [SerializeField] private GameObject zoomInButton;
        [SerializeField] private GameObject zoomOutButton;
        [SerializeField] private Button changeViewButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private GameObject profileCanvas;
        [SerializeField] private Button profileBackButton;
        [SerializeField] private Button weatherButton;
        [SerializeField] private Button weatherBackButton;
        [SerializeField] private GameObject weatherCanvas;

        [Tooltip("�ΰ����� ���� ���� ���׸����")]
        [SerializeField] private List<Material> materials;
        [Range(0.0f, 0.5f)]
        public float filterStrenth;

        void Start()
        {
            eventSystem = EventSystem.current;
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            overlookTransform = GameObject.Find("OverlookCamPos").GetComponent<Transform>();
            eyeLevelTransform = GameObject.Find("EyeLevelCamPos").GetComponent<Transform>();
            PlayerGPSLocation = GameObject.Find("PlayerGPSLocation").GetComponent<Transform>();
            player = GameObject.Find("Player");


            layerMask = 1 << LayerMask.NameToLayer("Store");
            RenderSettings.fog = false;

            CurrentControl.overlookAction += SwitchCont;
            CurrentControl.eyelevelAction += SwitchCont;
            CurrentControl.searchResultAction += SwitchCont;
            CurrentControl.profileAction += SwitchCont;
            CurrentControl.weatherAction += SwitchCont;

            resetPosButton.onClick.AddListener(() => ResetPosToPlayer());
            profileButton.onClick.AddListener(() => CurrentControl.ChangeToProfile());
            profileBackButton.onClick.AddListener(() => CurrentControl.ChangeToLastState());
            changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToEyelevel());
            weatherButton.onClick.AddListener(() => CurrentControl.ChangeToWeather());
            weatherBackButton.onClick.AddListener(() =>  CurrentControl.ChangeToLastState());
        }

#if DEVELOPMENT
        private void OnGUI()
        {
            GUI.Label(new Rect(20, 500, 700, 300), $"{debugText}");
            GUI.Label(new Rect(20, 850, 700, 200), $"ray obj: {rayObj}");
        }
#endif

        void Update()
        {
            // ���� ���� ��尡 �ΰ��� ��
            if (CurrentControl.state == CurrentControl.State.Overlook)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        movePos = Vector3.zero;

                        // ��ġ�� UI������ ���۵� ��� ī�޶� �̵� false
                        if (IsPointerOverUI(touch.fingerId))
                            startOnUI01 = false;
                        else
                        {
                            startOnUI01 = true;
                            prePos = touch.position - touch.deltaPosition;
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // ȭ�� �̵� �ӵ� ������Ʈ
                        spanSpeed = CalSpanSpeed(mainCamera.fieldOfView);

                        if (startOnUI01)
                        {
                            // ���� ī�޶� �̵� ����
                            nowPos = touch.position - touch.deltaPosition;
                            movePos = (Vector3)(prePos - nowPos) * spanSpeed * Time.deltaTime;
                            camTarget.transform.Translate(movePos);
                            // mainCamera.transform.Translate(movePos);
                            prePos = touch.position - touch.deltaPosition;
                        }
                    }

                    // �� �հ��� ��Ʈ�� ����
                    if (Input.touchCount == 2)
                    {
                        Touch touch02 = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touch.position - touch.deltaPosition;
                        Vector2 touchOnePrevPos = touch02.position - touch02.deltaPosition;

                        if (touch02.phase == TouchPhase.Moved)
                        {
                            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                            float touchDeltaMag = (touch.position - touch02.position).magnitude;

                            float deltaDiff = prevTouchDeltaMag - touchDeltaMag;
                            var newFOV = cameras[0].m_Lens.FieldOfView += twoFingerZoomPower * deltaDiff * Time.deltaTime;
                            cameras[0].m_Lens.FieldOfView = Clamp(newFOV, minFOV, maxFOV);

                            filterStrenth = (newFOV - 50) / 70;
                            filterStrenth = Clamp(filterStrenth, 0.0f, 0.5f);
                            debugText = filterStrenth.ToString();
                            foreach (var item in materials)
                            {
                                item.SetFloat("_FilterStrenth", filterStrenth);
                            }
                        }
                    }
                }
            }
            // ���� ���� ��尡 ���̷����� ��
            else if (CurrentControl.state == CurrentControl.State.Eyelevel)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        // ��ġ�� UI������ ���۵� ��� ī�޶� �̵� false
                        if (IsPointerOverUI(touch.fingerId))
                            startOnUI01 = true;
                        else
                        {
                            startOnUI01 = false;
                            player.transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.Self);
                            prePos = touch.position - touch.deltaPosition;

                            // ù ��ġ �� ���̿� ���� ������Ʈ ����
                            GetEncounter(ref firstEncounter);
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        if (!startOnUI01)
                        {
                            // ���� ī�޶� ȸ�� ����
                            nowPos = touch.position - touch.deltaPosition;
                            movePos = (Vector3)(prePos - nowPos) * rotateSpeed * Time.deltaTime;

                            var verAnglePow = movePos.y * -1;
                            var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                            if (nextVerAngle > 180)
                                nextVerAngle -= 360;

                            if(-30 < nextVerAngle && nextVerAngle < 30)
                            {
                                eyeLevelTransform.rotation *= Quaternion.AngleAxis(verAnglePow, Vector3.right);
                            }

                            player.transform.Rotate(new Vector3(0, movePos.x, 0), Space.Self);
                            prePos = touch.position - touch.deltaPosition;

                            // ��ġ �̵� �� ���̿� ���� ������Ʈ ����
                            GetEncounter(ref lastEncounter);
                        }
                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        GetEncounter(ref lastEncounter);
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // ��ġ�� ���� ���� ������Ʈ�� ��ġ�� ������ ���� ������Ʈ�� ������ �˻�
                        if (CheckSameObject(firstEncounter, lastEncounter))
                        {

                        }

                        firstEncounter = null;
                        lastEncounter = null;
                    }

                    // �� ��° �հ��� ��Ʈ�� ����
                    if (Input.touchCount == 2)
                    {
                        Touch touch02 = Input.GetTouch(1);

                        if (touch02.phase == TouchPhase.Began)
                        {
                            // ��ġ�� UI������ ���۵� ��� ī�޶� �̵� false
                            if (IsPointerOverUI(touch02.fingerId))
                                startOnUI02 = true;
                            else
                            {
                                startOnUI02 = false;
                                player.transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.Self);
                                prePos02 = touch02.position - touch02.deltaPosition;
                            }
                        }
                        else if (touch02.phase == TouchPhase.Moved)
                        {
                            if (!startOnUI02)
                            {
                                // ���� ī�޶� ȸ�� ����
                                nowPos02 = touch.position - touch.deltaPosition;
                                movePos02 = (Vector3)(prePos02 - nowPos02) * rotateSpeed * Time.deltaTime;

                                var verAnglePow = movePos02.y * -1;
                                var nextVerAngle = eyeLevelTransform.eulerAngles.x + verAnglePow;
                                if (nextVerAngle > 180)
                                    nextVerAngle -= 360;

                                if (-30 < nextVerAngle && nextVerAngle < 30)
                                {
                                    eyeLevelTransform.rotation *= Quaternion.AngleAxis(verAnglePow, Vector3.right);
                                }

                                player.transform.Rotate(new Vector3(0, movePos02.x, 0), Space.Self);
                                prePos02 = touch.position - touch.deltaPosition;

                                // ��ġ �̵� �� ���̿� ���� ������Ʈ ����
                                GetEncounter(ref lastEncounter);
                            }
                            debugText = $"nowpos2:{nowPos02}";
                        }
                    }
                }
            }
        }

        // ī�޶� ����
        public void CamZoomIn()
        {
            var tempFOV = Clamp(cameras[0].m_Lens.FieldOfView - 10, minFOV, maxFOV);
            var moveFOV = Abs(cameras[0].m_Lens.FieldOfView) - Abs(tempFOV);
            StartCoroutine(CamMove((int)moveFOV));
        }

        // ī�޶� �ܾƿ�
        public void CamZoomOut()
        {
            var tempFOV = Clamp(cameras[0].m_Lens.FieldOfView + 10, minFOV, maxFOV);
            var moveFOV = Abs(cameras[0].m_Lens.FieldOfView) - Abs(tempFOV);
            StartCoroutine(CamMove((int)moveFOV));
        }

        // ī�޶� �� �ڷ�ƾ
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
        /// ��Ʈ�� ���� ��ȭ �ÿ� ȣ��� �޼ҵ�
        /// </summary>
        public void SwitchCont()
        {
            if (CurrentControl.state == CurrentControl.State.Overlook)
            {
                //mainCamera.transform.parent = null;
                //mainCamera.transform.position = overlookTransform.position;
                //mainCamera.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

                SwitchCamera(cameras[0]);

                joyStick.SetActive(false);
                sideButton.SetActive(true);
                zoomInButton.SetActive(true);
                zoomOutButton.SetActive(true);

                changeViewButton.onClick.RemoveAllListeners();
                changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToEyelevel());

                RenderSettings.fog = false;
                profileButton.gameObject.SetActive(true);
                profileCanvas.SetActive(false);
                weatherCanvas.SetActive(false);
            }
            else if (CurrentControl.state == CurrentControl.State.Eyelevel)
            {
                //mainCamera.transform.parent = eyeLevelTransform;
                //mainCamera.transform.localPosition = Vector3.zero;
                //mainCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
                //mainCamera.fieldOfView = defaultFOV;

                SwitchCamera(cameras[1]);

                joyStick.SetActive(true);
                sideButton.SetActive(true);
                zoomInButton.SetActive(false);
                zoomOutButton.SetActive(false);

                changeViewButton.onClick.RemoveAllListeners();
                changeViewButton.onClick.AddListener(() => CurrentControl.ChangeToOverlook());

                StartCoroutine(WaitThenCallback(0.7f,
                    () => { if(CurrentControl.state == CurrentControl.State.Eyelevel)
                        {
                            RenderSettings.fog = true;
                            RenderSettings.fogDensity = 0.005f;
                        } 
                        
                    }));
                
                profileButton.gameObject.SetActive(true);
                profileCanvas.SetActive(false);
                weatherCanvas.SetActive(false);
            }
            else if (CurrentControl.state == CurrentControl.State.SearchResult)
            {
                joyStick.SetActive(false);
                sideButton.SetActive(false);

                profileCanvas.SetActive(false);
            }
            else if (CurrentControl.state == CurrentControl.State.Profile)
            {
                joyStick.SetActive(false);
                sideButton.SetActive(false);
                profileButton.gameObject.SetActive(false);

                profileCanvas.SetActive(true);
            }
            else if (CurrentControl.state == CurrentControl.State.Weather)
            {
                joyStick.SetActive(false);
                sideButton.SetActive(false);
                profileButton.gameObject.SetActive(false);

                weatherCanvas.SetActive(true);
            }
        }

        private IEnumerator WaitThenCallback(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback();
        }

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

        // ���� ����� ��ġ�� ī�޶� �̵�
        public void ResetPosToPlayer()
        {
            if (CurrentControl.state == CurrentControl.State.Overlook)
            {
                // mainCamera.transform.parent = null;
                camTarget.transform.position = PlayerGPSLocation.transform.position;
                //mainCamera.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
            else if (CurrentControl.state == CurrentControl.State.Eyelevel)
            {
                player.transform.position = PlayerGPSLocation.transform.position;
            }
        }

        /// <summary>
        /// ���̿� ���� ������Ʈ�� ��ȯ, �ִ� �Ÿ��� ���̾��ũ�� �̹� �ۼ��Ǿ� ����
        /// </summary>
        /// <param name="output"></param>
        private void GetEncounter(ref GameObject output)
        {
#if UNITY_EDITOR
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, maxRayDis, layerMask))
            {
                rayObj = hit.collider.gameObject.name;
                output = hit.collider.gameObject;
            }
            else
            {
                output = null;
            }
#elif UNITY_ANDROID
        
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if(Physics.Raycast(ray, out hit, maxRayDis, layerMask))
        { 
            rayObj = hit.collider.gameObject.name;
            output = hit.collider.gameObject;
        }
        else
        {
            output = null;
        }
#elif UNITY_IOS
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        if(Physics.Raycast(ray, out hit, maxRayDis, layerMask)) 
        { 
            rayObj = hit.collider.gameObject.name;
            output = hit.collider.gameObject;
        }
        else
        {
            output = null;
        }
#endif
        }

        private bool CheckSameObject(GameObject objA, GameObject objB)
        {
            if (objA == null || objB == null)
            {
                return false;
            }
            else
            {
                if (objA == objB)
                    return true;
                else
                    return false;
            }
        }

        public void GetInput(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
        }

        // ��ġ�� UI������ �̷�������� �˻�
        public bool IsPointerOverUI(int fingerId)
        {
            return (eventSystem.IsPointerOverGameObject(fingerId) && eventSystem.currentSelectedGameObject != null);
        }
    }
}