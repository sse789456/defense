using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab; // 체력바 프리팹
    private Image healthBarImage; // 체력바 이미지
    private GameObject healthBarInstance; // 체력바 인스턴스

    public float maxHealth = 100f; // 최대 체력
    private float currentHealth; // 현재 체력

    public Vector3 offset = new Vector3(0, 2, 0); // 대상 오브젝트 머리 위로의 오프셋

    void Start()
    {
        // 초기 체력 값을 최대 체력으로 설정
        currentHealth = maxHealth;

        // 체력바 인스턴스를 생성하고 설정
        CreateHealthBar();
        UpdateHealthBar();
    }

    // 체력바 인스턴스를 생성하는 메서드
    private void CreateHealthBar()
    {
        if (healthBarPrefab != null)
        {
            // 체력바 인스턴스를 생성하고 부모를 이 오브젝트로 설정
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarInstance.transform.localPosition = offset; // 오프셋 적용
            healthBarImage = healthBarInstance.GetComponentInChildren<Image>();

            // 체력바를 월드 공간에서 UI 캔버스 안에 배치
            Canvas canvas = healthBarInstance.GetComponentInChildren<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
        }
    }

    // 체력을 설정하는 메서드
    public void SetHealth(float health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth); // 체력을 0과 최대 체력 사이로 제한
        UpdateHealthBar();
    }

    // 체력을 감소시키는 메서드
    public void TakeDamage(float damage)
    {
        SetHealth(currentHealth - damage);
    }

    // 체력을 회복시키는 메서드
    public void Heal(float amount)
    {
        SetHealth(currentHealth + amount);
    }

    // 체력바를 업데이트하는 메서드
    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            // 현재 체력에 따라 체력바 이미지의 fillAmount를 업데이트
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }
    }

    // 오브젝트가 제거될 때 체력바 인스턴스도 제거
    void OnDestroy()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance);
        }
    }
}