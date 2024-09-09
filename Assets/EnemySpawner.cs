using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // 스폰할 적 프리팹
    public float minSpawnInterval = 2f; // 최소 스폰 간격
    public float maxSpawnInterval = 5f; // 최대 스폰 간격
    public float spawnRadius = 5f; // 스폰 반경

    private float _timeSinceLastSpawn;
    private float _currentSpawnInterval;

    void Start()
    {
        // 시작할 때 초기 스폰 간격을 설정합니다.
        SetRandomSpawnInterval();
    }

    void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn >= _currentSpawnInterval)
        {
            SpawnEnemy();
            _timeSinceLastSpawn = 0f;
            SetRandomSpawnInterval(); // 스폰할 때마다 새로운 스폰 간격을 설정합니다.
        }
    }

    private void SetRandomSpawnInterval()
    {
        _currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void SpawnEnemy()
    {
        // 스포너 오브젝트를 기준으로 스폰 반경 내의 랜덤 위치를 계산합니다.
        Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
        randomPosition.y = 0; // y축 위치를 0으로 설정하여 평면 상에서 스폰되도록 합니다.
        Vector3 spawnPosition = transform.position + randomPosition;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}