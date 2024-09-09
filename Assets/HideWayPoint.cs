using UnityEngine;

public class HideWayPoint : MonoBehaviour
{
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        // 자식 오브젝트들도 비활성화
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childRenderers)
        {
            childRenderer.enabled = false;
        }
    }
}