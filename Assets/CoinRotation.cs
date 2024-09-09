using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    public float rotationSpeed = 180f;
    public float collectionDistance = 1f;
    private Rigidbody rb;
    private GameObject newChan;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ |
                             RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        // "NewChan" 태그를 가진 오브젝트를 찾습니다.
        newChan = GameObject.FindGameObjectWithTag("NewChan");
    }

    void Update()
    {
        // 코인 회전
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // NewChan과의 거리 체크 및 수집
        if (newChan != null)
        {
            float distance = Vector3.Distance(transform.position, newChan.transform.position);
            if (distance <= collectionDistance)
            {
                CollectCoin();
            }
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Y축 속도만 유지하고 X, Z축 속도는 0으로 설정
            Vector3 velocity = rb.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            rb.velocity = velocity;
        }
    }

    void CollectCoin()
    {
        // GameManager를 통해 코인 수집 처리
        GameManager.Instance.CollectCoin();

        // 효과음 재생 (만약 있다면)
        // AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // 코인 오브젝트 제거
        Destroy(gameObject);
    }
}