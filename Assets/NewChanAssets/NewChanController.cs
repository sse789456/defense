using UnityEngine;
using System.Collections;

public class NewchanController : MonoBehaviour
{
    public GameObject newchan; // 이동과 회전을 담당하는 오브젝트
    public float rotationSpeed = 100.0f; // 회전 속도 설정 (단위: 도/초)
    public float moveSpeed = 5.0f; // 이동 속도 설정 (단위: 유니티 단위/초)
    public float jumpForce = 10.0f; // 점프 힘 설정
    public float speedThreshold = 0.3f; // 애니메이터 스레쉬홀드 설정
    public float jumpCooldown = 1.0f; // 점프 후 Speed 값이 변경되지 않을 시간 (초)
    public float extraGravity = 20.0f; // 추가 중력 값

    private Rigidbody rb; // Rigidbody 컴포넌트
    private Animator animator; // Animator 컴포넌트
    private bool isJumping = false; // 점프 상태를 관리하기 위한 변수
    private bool isCannonActive = false; // 캐논이 활성화되어 있는지 여부

    void Start()
    {
        rb = newchan.GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        animator = newchan.GetComponent<Animator>(); // Animator 컴포넌트 가져오기

        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on " + newchan.name);
        }

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + newchan.name);
        }
    }

    void Update()
    {
        if (isCannonActive)
        {
            return; // 캐논이 활성화되면 입력 처리를 건너뜀
        }

        // WASD 입력 감지 (앞으로 전진, 뒤로 후진, 좌우 회전)
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += newchan.transform.forward; // 전진
            if (!isJumping) // 점프 중이 아닐 때만 Speed를 변경
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", 0.3f); // W 키를 누를 때 애니메이터의 Speed 파라미터를 0.3으로 변경
                }
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= newchan.transform.forward; // 후진
            if (!isJumping) // 점프 중이 아닐 때만 Speed를 변경
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", -0.3f); // S 키를 누를 때 애니메이터의 Speed 파라미터를 -0.3으로 변경
                }
            }
        }

        if (moveDirection != Vector3.zero)
        {
            Vector3 moveAmount = moveDirection.normalized * moveSpeed * Time.deltaTime;
            rb.MovePosition(rb.position + moveAmount);
        }
        else
        {
            if (!isJumping && animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        // A와 D 키 입력 감지 (좌우 회전)
        if (Input.GetKey(KeyCode.A))
        {
            newchan.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime); // 좌회전
        }
        if (Input.GetKey(KeyCode.D))
        {
            newchan.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime); // 우회전
        }

        // 발사 입력 감지 (예: 스페이스바)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Jump()); // 점프 코루틴 시작
        }
    }

    private IEnumerator Jump()
    {
        if (isJumping)
        {
            yield break; // 이미 점프 중이면 추가 점프를 무시
        }

        if (animator != null)
        {
            animator.SetTrigger("Jump"); // 점프 트리거를 설정하여 점프 애니메이션 실행
        }

        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 점프 힘 추가
        }

        isJumping = true; // 점프 상태로 설정

        yield return new WaitForSeconds(jumpCooldown);

        isJumping = false; // 점프 상태 해제

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
        }
    }

    void FixedUpdate()
    {
        if (rb != null && !isJumping)
        {
            rb.AddForce(Vector3.down * extraGravity, ForceMode.Acceleration); // 추가 중력 적용
        }
    }

    // 캐논과의 충돌을 감지하는 메서드
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cannon"))
        {
            isCannonActive = true;
            Debug.Log("Collided with Cannon. Disabling Newchan controls.");
        }
    }

    // 제어 상태를 설정하는 메서드
    public void SetControlEnabled(bool enabled)
    {
        isCannonActive = !enabled;
    }

    // 캐논 비활성화 시 호출되는 메서드
    public void OnCannonDeactivated()
    {
        isCannonActive = false;
        Debug.Log("Cannon deactivated. Re-enabling Newchan controls.");
    }
}
