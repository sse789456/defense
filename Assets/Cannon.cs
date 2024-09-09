using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    private GameObject cannonReadyPrefab; // CannonReady 프리팹 설정
    [SerializeField]
    private GameObject smallCannon; // 이동과 회전을 담당하는 오브젝트
    [SerializeField]
    private GameObject cannonBallSpawnPoint; // 발사 위치 및 각도 조정 오브젝트
    [SerializeField]
    private GameObject cannonBallPrefab; // CannonBall 프리팹 설정
    [SerializeField]
    private float rotationSpeed = 10.0f; // 회전 속도 설정
    [SerializeField]
    private float launchSpeed = 10.0f; // 발사 속도 설정
    [SerializeField]
    private float moveSpeed = 5.0f; // 이동 속도 설정
    [SerializeField]
    private Camera cannonCamera; // 캐논 조작용 카메라
    [SerializeField]
    private NewchanController newchanController; // NewchanController 참조

    private bool isCannonActive = false; // 캐논 활성화 상태
    private bool hasCollidedWithNewChan = false; // NewChan과 충돌했는지 여부
    private Camera mainCamera; // 메인 카메라

    private void Start()
    {
        mainCamera = Camera.main; // 메인 카메라 참조
        if (cannonCamera != null)
        {
            cannonCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isCannonActive) return;

        HandleCannonControls();
    }

    private void HandleCannonControls()
    {
        // 마우스 휠 입력 감지
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScroll != 0)
        {
            Vector3 currentRotation = cannonBallSpawnPoint.transform.localEulerAngles;
            currentRotation.x -= mouseScroll * rotationSpeed;
            cannonBallSpawnPoint.transform.localEulerAngles = currentRotation;

            Vector3 cameraRotation = cannonCamera.transform.localEulerAngles;
            cameraRotation.x -= mouseScroll * rotationSpeed;
            cannonCamera.transform.localEulerAngles = cameraRotation;
        }

        // WASD 입력 감지
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += smallCannon.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= smallCannon.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            smallCannon.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            smallCannon.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // 이동 처리
        Vector3 moveAmount = moveDirection.normalized * moveSpeed * Time.deltaTime;
        smallCannon.transform.position += moveAmount;

        // 발사 입력 감지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchCannonBall();
        }
    }

    private void LaunchCannonBall()
    {
        Vector3 spawnPosition = cannonBallSpawnPoint.transform.position + cannonBallSpawnPoint.transform.forward * 3.0f;
        GameObject cannonBall = Instantiate(cannonBallPrefab, spawnPosition, cannonBallSpawnPoint.transform.rotation);
        Rigidbody rb = cannonBall.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = cannonBallSpawnPoint.transform.forward * launchSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("NewChan"))
        {
            Debug.Log("Cannon collided with NewChan!");
            hasCollidedWithNewChan = true;
        }
    }

    private void OnEnable()
    {
        isCannonActive = false;
        hasCollidedWithNewChan = false;
    }

    private void OnGUI()
    {
        if (hasCollidedWithNewChan && !isCannonActive)
        {
            if (GUI.Button(new Rect(10, 10, 150, 50), "Activate Cannon"))
            {
                ActivateCannon();
            }
        }
        if (isCannonActive)
        {
            if (GUI.Button(new Rect(10, 70, 150, 50), "Deactivate Cannon"))
            {
                DeactivateCannon();
            }
        }
    }

    private void ActivateCannon()
    {
        isCannonActive = true;

        if (cannonCamera != null)
        {
            cannonCamera.gameObject.SetActive(true);
            Debug.Log("Cannon camera activated: " + cannonCamera.gameObject.name);
        }
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(false);
            Debug.Log("Main camera deactivated: " + mainCamera.gameObject.name);
        }

        // NewchanController의 제어 상태 업데이트
        if (newchanController != null)
        {
            newchanController.SetControlEnabled(false);
        }

        Debug.Log("Cannon activated: " + gameObject.name);
    }

    private void DeactivateCannon()
    {
        isCannonActive = false;

        if (cannonCamera != null)
        {
            cannonCamera.gameObject.SetActive(false);
            Debug.Log("Cannon camera deactivated: " + cannonCamera.gameObject.name);
        }
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(true);
            Debug.Log("Main camera activated: " + mainCamera.gameObject.name);
        }

        // NewchanController의 제어 상태 업데이트
        if (newchanController != null)
        {
            newchanController.SetControlEnabled(true);
        }

        Debug.Log("Cannon deactivated: " + gameObject.name);
    }
}
