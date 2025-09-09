using UnityEngine;

public class DestructibleObstacle : MonoBehaviour
{
    [SerializeField] private GameObject destroyedPrefab;

    public void DestroyObstacle()
    {
        if (destroyedPrefab != null)
            Instantiate(destroyedPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
