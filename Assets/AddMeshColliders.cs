using UnityEngine;

public class AddMeshColliders : MonoBehaviour
{
    void Start()
    {
        AddCollidersRecursively(transform);
    }

    void AddCollidersRecursively(Transform parent)
    {
        foreach (Transform child in parent)
        {
            MeshFilter meshFilter = child.GetComponent<MeshFilter>();
            if (meshFilter != null && child.GetComponent<MeshCollider>() == null)
            {
                MeshCollider meshCollider = child.gameObject.AddComponent<MeshCollider>();
                meshCollider.convex = true; 
            }
            AddCollidersRecursively(child);
        }
    }
}