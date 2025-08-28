using UnityEngine;

public class Arrow : MonoBehaviour
{
    public TPVPlayerCombat.ArrowType arrowType;

    public void SetArrowType(TPVPlayerCombat.ArrowType type)
    {
        arrowType = type;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Arrow collided with: {collision.collider.name}");
        var interactive = collision.collider.GetComponent<IArrowInteractable>();
        if (interactive != null)
        {
            interactive.OnArrowHit(arrowType);
            Debug.Log($"Arrow hit interactable object: {collision.collider.name}");
        }
        else
        {
            Debug.Log("Arrow hit non-interactable object");
        }

        Destroy(gameObject);
    }
}
