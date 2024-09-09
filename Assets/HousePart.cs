using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePart : MonoBehaviour
{
    public GameObject prefabToSpawn; // 인스펙터에서 할당할 프리팹
    private List<Rigidbody> _partsRigidbodies = new List<Rigidbody>();
    private bool _onHitted;
    private Coroutine _spawnCoroutine;

    void Start()
    {
        _onHitted = false;
        
        // 자식 GameObject들에게 Rigidbody와 MeshCollider를 추가
        AddComponentsToChildrenRecursive(transform);

        if (prefabToSpawn != null)
        {
            // 5초마다 오브젝트 소환 시작
            _spawnCoroutine = StartCoroutine(SpawnObjectsEveryFiveSeconds());
        }
        else
        {
            Debug.LogError("PrefabToSpawn is not assigned.");
        }
    }

    void AddComponentsToChildrenRecursive(Transform parent)
    {
        // 현재 부모 GameObject에 Rigidbody와 MeshCollider 추가
        Rigidbody rb = parent.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.gameObject.layer = LayerMask.NameToLayer("House");
        _partsRigidbodies.Add(rb);

        MeshCollider meshCollider = parent.gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.isTrigger = false;

        HousePartCollisionController controller = parent.gameObject.AddComponent<HousePartCollisionController>();
        controller.housePart = this;

        // 자식 GameObject가 없으면 함수 종료
        if (parent.childCount == 0)
            return;

        // 모든 자식 GameObject에 재귀적으로 함수 호출
        foreach (Transform child in parent)
        {
            AddComponentsToChildrenRecursive(child);
        }
    }

    IEnumerator SpawnObjectsEveryFiveSeconds()
    {
        while (!_onHitted)
        {
            yield return new WaitForSeconds(7f);
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (prefabToSpawn != null)
        {
            // 프리팹을 소환
            Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            
        }
    }

    public void OnCollisionEnter_Custom(Collision other)
    {
        if (!other.gameObject.CompareTag("CannonBall"))
        {
            return;
        }
        

        if (_onHitted)
        {
            return;
        }

        foreach (var partsRigidbody in _partsRigidbodies)
        {
            partsRigidbody.isKinematic = false;
        }

        _onHitted = true;

        // 충돌 시 코루틴 중지
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
        }
    }
}
