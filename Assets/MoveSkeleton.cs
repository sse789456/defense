using UnityEngine;

public class MoveSkeleton : MonoBehaviour
{
    public float moveSpeed = 10f; // 이동 속도 설정
    public Transform[] waypoints; // 웨이포인트 배열
    private int currentWaypointIndex = 0; // 현재 웨이포인트 인덱스
    private bool isAtLastWaypoint = false; // 마지막 웨이포인트 도착 여부

    private Animator animator; // 애니메이터 컴포넌트
    private Rigidbody rb; // Rigidbody 컴포넌트

    void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기

        // animator 또는 rb가 null인지 확인하고 로그 출력
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on " + gameObject.name);
        }

        // 웨이포인트가 설정되어 있지 않으면 로그 출력
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Waypoints not set on " + gameObject.name);
        }
    }

    void Update()
    {
        if (waypoints != null && waypoints.Length > 0)
        {
            if (!isAtLastWaypoint)
            {
                MoveToWaypoint();
            }
            else
            {
                // 마지막 웨이포인트에 도착하면 자유롭게 이동
                AllowFreeMovement();
            }
        }
    }

    void MoveToWaypoint()
    {
        // 현재 웨이포인트를 가져옴
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // 현재 위치와 목표 위치 간의 방향 벡터를 계산
        Vector3 directionToTarget = (targetWaypoint.position - transform.position).normalized;

        // X와 Z 축의 회전을 고정하기 위해 Y 축만 사용하여 회전 벡터 계산
        directionToTarget.y = 0; // Y축 회전만 고려
        if (directionToTarget != Vector3.zero) // 방향이 0이 아닌 경우에만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }

        // 웨이포인트 방향으로 이동
        Vector3 moveDirection = (targetWaypoint.position - transform.position).normalized;
        Vector3 moveAmount = moveDirection * moveSpeed * Time.deltaTime;
        transform.position += moveAmount;

        // 이동량이 0이 아닌 경우에만 이동 애니메이션 재생
        if (moveAmount.magnitude > 0)
        {
            animator.SetFloat("MoveSpeed", moveSpeed); // 이동 애니메이션 재생을 위해 MoveSpeed 파라미터 설정
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0f); // 정지할 때는 MoveSpeed를 0으로 설정하여 이동 애니메이션 중지
        }

        // 웨이포인트에 도착했는지 확인
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // 다음 웨이포인트로 변경
            currentWaypointIndex++;

            // 마지막 웨이포인트에 도달하면 이동 속도와 회전 설정을 변경
            if (currentWaypointIndex >= waypoints.Length)
            {
                isAtLastWaypoint = true;
                moveSpeed = 15f; // 마지막 웨이포인트에 도달하면 이동 속도를 15로 설정
                animator.SetFloat("MoveSpeed", moveSpeed); // 이동 애니메이션 속도 업데이트

                // Rigidbody의 이동 제약을 해제하여 자유롭게 이동하도록 설정
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }

    void AllowFreeMovement()
    {
        // 마지막 웨이포인트에 도착한 후 자유롭게 이동
        // 이동 방향을 현재 방향으로 설정
        Vector3 moveDirection = transform.forward; // 현재 방향으로 이동
        Vector3 moveAmount = moveDirection * moveSpeed * Time.deltaTime;
        transform.position += moveAmount;

        // 애니메이션과 이동 속도 업데이트
        animator.SetFloat("MoveSpeed", moveSpeed); // 계속 이동 애니메이션 재생
    }

    void OnCollisionEnter(Collision collision)
    {
        if (animator == null || rb == null) return;

        // 충돌한 객체의 태그가 "Skeleton"일 경우 충돌을 무시합니다.
        if (collision.gameObject.CompareTag("Skeleton"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            return;
        }

        // 충돌한 객체의 레이어를 가져옵니다.
        int layer = collision.gameObject.layer;
        string layerName = LayerMask.LayerToName(layer);

        // 충돌한 객체의 레이어가 "Wall"일 경우에만 회전합니다.
        if (layerName == "Wall")
        {
            // 현재 객체의 회전 각도에 80도를 추가합니다.
            transform.Rotate(Vector3.up, 80f);
        }

        // 충돌한 객체의 태그가 "CannonBall"일 경우
        if (collision.gameObject.CompareTag("CannonBall"))
        {
            // 이동 속도를 0으로 설정합니다.
            moveSpeed = 0f;

            // Animator의 MoveSpeed 파라미터를 0으로 설정하여 이동 애니메이션을 중지합니다.
            animator.SetFloat("MoveSpeed", 0f);

            // Rigidbody의 이동 제약을 해제하여 자유롭게 이동하도록 설정
            // rb.constraints = RigidbodyConstraints.FreezeRotationX;
            // rb.constraints = RigidbodyConstraints.FreezeRotationY; 
            // rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
    }
}