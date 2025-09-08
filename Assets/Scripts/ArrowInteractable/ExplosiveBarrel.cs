using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosiveBarrel : MonoBehaviour, IArrowInteractable
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float delayBeforeExplosion = 0.5f;
    [SerializeField] private GameObject explosionVFX;

    [Header("Barrel Chain Settings")]
    [SerializeField] private LayerMask barrelLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private List<ExplosiveBarrel> linkedBarrels;

    [Header("Platform Settings")]
    [SerializeField] private GameObject platformToRise;
    [SerializeField] private Transform platformFinalPlacement;
    [SerializeField] private float platformMoveSpeed = 2f;

    private bool hasExploded = false;

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        if (arrowType == TPVPlayerCombat.ArrowType.Pyro && !hasExploded)
            StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionVFX != null)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearby in colliders)
        {
            Rigidbody rb = nearby.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            ExplosiveBarrel barrel = nearby.GetComponent<ExplosiveBarrel>();
            if (barrel != null && barrel != this && linkedBarrels.Contains(barrel))
                barrel.OnArrowHit(TPVPlayerCombat.ArrowType.Pyro);

            if (((1 << nearby.gameObject.layer) & obstacleLayerMask) != 0)
            {
                DestructibleObstacle obs = nearby.GetComponent<DestructibleObstacle>();
                if (obs != null)
                    obs.DestroyObstacle();
            }
        }

        CheckAllBarrelsExploded();
        Destroy(gameObject);
    }

    private void CheckAllBarrelsExploded()
    {
        foreach (var barrel in linkedBarrels)
        {
            if (barrel != null && !barrel.hasExploded)
                return;
        }

        if (platformToRise != null && platformFinalPlacement != null)
            StartCoroutine(RaisePlatform());
    }

    private IEnumerator RaisePlatform()
    {
        Vector3 startPos = platformToRise.transform.position;
        Vector3 endPos = new Vector3(startPos.x, platformFinalPlacement.position.y, startPos.z);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * platformMoveSpeed;
            platformToRise.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        platformToRise.transform.position = endPos;
    }
}
