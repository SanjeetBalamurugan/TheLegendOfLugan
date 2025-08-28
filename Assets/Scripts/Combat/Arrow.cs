using UnityEngine;

public class Arrow : MonoBehaviour
{
    private TPVPlayerCombat.ArrowType arrowType;

    public void SetArrowType(TPVPlayerCombat.ArrowType type)
    {
        arrowType = type;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var interactive = collision.collider.GetComponent<IArrowInteractable>();
        if (interactive != null)
        {
            interactive.OnArrowHit(arrowType);
        }

        Destroy(gameObject);
    }
}
