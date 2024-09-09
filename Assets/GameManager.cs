using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private static readonly object _lock = new object();
    
    public float money = 1000f;
    public float currentCoinValue = 30f;
    public TextMeshProUGUI moneyText;
    
    public float currentExp = 0f;
    public float maxExp = 100f;
    public TextMeshProUGUI expText;
    public Image expBarFill;
    
    public GameObject levelUpEffectPrefab; // 레벨업 이펙트 프리팹
    public Transform effectSpawnPoint; // 이펙트 생성 위치 (선택적)
    
    private int currentLevel = 1; // 현재 레벨 추가

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<GameManager>();
                        
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject("GameManager");
                            _instance = singleton.AddComponent<GameManager>();
                        }
                    }
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        UpdateMoneyUI();
        UpdateExpUI();
    }

    // 돈 차감
    public bool SpendMoney(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
            return true;
        }
        return false; 
    }

    // 돈 추가 (코인 수집)
    public void CollectCoin()
    {
        money += currentCoinValue;
        UpdateMoneyUI();
    }

    // 돈 업데이트
    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "Money: " + money.ToString("F2");
        }
    }
    public void AddExperience(float expAmount)
    {
        currentExp += expAmount;
        
        bool leveledUp = false;
        while (currentExp >= maxExp)
        {
            // 레벨업
            currentLevel++;
            currentExp -= maxExp;
            maxExp *= 1.05f; // 5% 증가
            leveledUp = true;
        }

        UpdateExpUI();

        if (leveledUp)
        {
            ShowLevelUpEffect();
        }
    }

    // 레벨업 이펙트 표시
    private void ShowLevelUpEffect()
    {
        if (levelUpEffectPrefab != null)
        {
            Vector3 spawnPosition = effectSpawnPoint != null ? effectSpawnPoint.position : Camera.main.transform.position + Camera.main.transform.forward * 2f;
            GameObject effect = Instantiate(levelUpEffectPrefab, spawnPosition, Quaternion.identity);
            
            // 이펙트 지속 시간 (필요에 따라 조정)
            float effectDuration = 2f;
            Destroy(effect, effectDuration);
        }
    }
    private void UpdateExpUI()
    {
        float expPercentage = (currentExp / maxExp) * 100f;
        
        if (expText != null)
        {
            expText.text = $"Level: {currentLevel} - EXP: {expPercentage:F1}% ({currentExp:F0}/{maxExp:F0})";
        }

        if (expBarFill != null)
        {
            expBarFill.fillAmount = currentExp / maxExp;
        }
    }
}