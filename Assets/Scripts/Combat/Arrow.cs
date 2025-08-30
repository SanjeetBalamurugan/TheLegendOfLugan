using UnityEngine;

public class Arrow : MonoBehaviour
{
    public TPVPlayerCombat.ArrowType arrowType;

    [Header("Collision Settings")]
    [SerializeField] private bool useRaycastDetection = false; 
    [SerializeField] private LayerMask hitLayers = ~0; // everything by default

    private Vector3 lastPos;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        lastPos = transform.position;
    }

    private void Update()
    {
        if (useRaycastDetection)
        {
            Vector3 currentPos = transform.position;
            Vector3 direction = currentPos - lastPos;
            float distance = direction.magnitude;

            if (distance > 0f && Physics.Raycast(lastPos, direction.normalized, out RaycastHit hit, distance, hitLayers))
            {
                OnArrowImpact(hit.collider, hit.point);
            }

            lastPos = currentPos;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!useRaycastDetection)
        {
            OnArrowImpact(collision.collider, collision.contacts[0].point);
        }
    }

    private void OnArrowImpact(Collider collider, Vector3 hitPoint)
    {
        Debug.Log($"Arrow collided with: {collider.name}");
        var interactive = collider.GetComponent<IArrowInteractable>();
        if (interactive != null)
        {
            interactive.OnArrowHit(arrowType);
            Debug.Log($"Arrow hit interactable object: {collider.name}");
        }
        else
        {
            Debug.Log("Arrow hit non-interactable object");
        }

        Destroy(gameObject);
    }

    public void SetArrowType(TPVPlayerCombat.ArrowType type)
    {
        arrowType = type;
    }
}
