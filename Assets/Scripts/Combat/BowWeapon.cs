using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;         // where arrow spawns
    [SerializeField] private GameObject arrowPrefab;      // projectile prefab
    [SerializeField] private TPVPlayerMove playerMove;    // reference to player script

    [Header("Arrow Settings")]
    [SerializeField] private float arrowSpeed = 50f;
    [SerializeField] private float destroyAfterSeconds = 5f;

    [Header("Debug")]
    [SerializeField] private bool debugDraw = true;

    private void Update()
    {
        if (playerMove == null) return;

        // Aim direction
        Vector3 aimDir = (playerMove.GetAimTarget().position - firePoint.position).normalized;

        // Debug ray
        if (debugDraw)
        {
            Debug.DrawRay(firePoint.position, aimDir * 20f, Color.red);
            Debug.DrawLine(firePoint.position, playerMove.GetAimTarget().position, Color.green);
        }

        // Fire arrow
        if (Input.GetButtonDown("Fire1") && playerMove.GetAimValue())
        {
            ShootArrow(aimDir);
        }
    }

    private void ShootArrow(Vector3 direction)
    {
        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = direction * arrowSpeed;
        }

        Destroy(arrow, destroyAfterSeconds);
    }
}
