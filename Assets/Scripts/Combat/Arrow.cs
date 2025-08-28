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
        Debug.Log(collision.collider.name);
        var interactive = collision.collider.GetComponent<IArrowInteractable>();
        if (interactive != null)
        {
            interactive.OnArrowHit(arrowType);
        }

        Destroy(gameObject);
    }
}
