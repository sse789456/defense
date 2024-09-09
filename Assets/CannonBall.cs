using UnityEngine;

public class CannonBall : MonoBehaviour
{
    private bool isDragging = false; // 드래그 중인지 여부
    private Vector3 offset; // 마우스 클릭 시 대포알과 마우스 위치의 거리 차이
    
    void OnMouseDown()
    {
        // 마우스 클릭 시 시작 위치 설정
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }
    
    void OnMouseUp()
    {
        // 마우스 뗄 시 드래그 종료
        isDragging = false;
        
        // Rigidbody의 중력을 원래대로 설정
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // 물리 작용 가능하도록 설정
        rb.useGravity = true; // 중력 사용
    }

    void Update()
    {
        if (isDragging)
        {
            // 현재 마우스 위치에서 대포알의 위치로의 방향 벡터 계산
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            direction.z = 0f; // 2D에서 z 축은 사용하지 않음
            
            // 대포알 위치 업데이트
            transform.position = mousePosition + offset;
            
            // Rigidbody의 중력을 임시로 해제
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true; // 물리 작용 안함
            rb.useGravity = false; // 중력 미사용
        }
    }
}