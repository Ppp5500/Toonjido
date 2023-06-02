using UnityEngine;

namespace ToonJido.Control
{
    // 걍 이거 CameraWork에 합쳐버리는 게 나을 것 같은디?
    public class JoyStickMove : MonoBehaviour
    {
        private Vector3 moveInput;
        public GameObject cam;
        public float moveSpeed = 5f;

        private void Update()
        {
            transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.Self);
        }

        public void InputTest(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
        }
    }
}