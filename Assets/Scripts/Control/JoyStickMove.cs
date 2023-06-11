using UnityEngine;

namespace ToonJido.Control
{
    // �� �̰� CameraWork�� ���Ĺ����� �� ���� �� ������?
    public class JoyStickMove : MonoBehaviour
    {
        private Vector3 moveInput;
        public GameObject cam;
        public float moveSpeed = 5f;

        private void Update()
        {
            transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.Self);
        }

        public void GetInput(Vector2 input)
        {
            moveInput.x = input.x;
            moveInput.z = input.y;
        }
    }
}