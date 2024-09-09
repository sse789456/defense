using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // 카메라 이동 속도
    public float scrollSpeed = 10f; // 마우스 스크롤 속도
    public float rotationSpeed = 5f; // 마우스 회전 속도

    void Update()
    {
        // WASD 키 입력
        float horizontal = Input.GetAxis("Horizontal"); // A, D 또는 왼쪽, 오른쪽 화살표 키
        float vertical = Input.GetAxis("Vertical"); // W, S 또는 위, 아래 화살표 키

        // 카메라 이동
        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.Self);

        // 마우스 스크롤 입력 (카메라 줌인/줌아웃)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoom = transform.forward * scroll * scrollSpeed * Time.deltaTime;
        transform.Translate(zoom, Space.World);

        // 마우스 오른쪽 버튼 누른 상태에서 카메라 회전
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y"); // Y 축 방향 반전

            // 마우스 움직임에 따른 카메라 회전
            Vector3 rotation = new Vector3(mouseY, mouseX, 0) * rotationSpeed;
            transform.eulerAngles += rotation;
        }
    }
}