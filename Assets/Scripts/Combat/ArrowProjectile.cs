using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public TPVPlayerCombat.ArrowType arrowType;

    private void OnCollisionEnter(Collision collision)
    {
        IArrowInteractable interactable = collision.collider.GetComponent<IArrowInteractable>();
        if (interactable != null)
        {
            interactable.OnArrowHit(arrowType);
        }

        // Optionally destroy arrow after impact
        Destroy(gameObject, 2f);
    }
}
