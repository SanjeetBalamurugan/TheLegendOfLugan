using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private TPVPlayerCombat combat;
    [SerializeField] private TPVPlayerMove move;

    [Header("Arrow Settings")]
    [SerializeField] private float arrowSpeed = 30f;
    [SerializeField] private float maxChargeTime = 2f;

    [Header("Aiming")]
    [SerializeField] private float aimMaxDistance = 1000f;
    [SerializeField] private LayerMask aimLayerMask = ~0;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private bool isAiming;
    private float chargeTime;

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (isAiming)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime);

            if (Input.GetMouseButtonDown(0))
            {
                TryFireArrow();
                chargeTime = 0f;
            }
        }
        else
        {
            chargeTime = 0f;
        }
    }

    private void TryFireArrow()
    {
        if (!combat.HasArrows()) return;
        if (combat.TryConsumeArrow()) FireArrow();
    }

    private void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        float chargePercent = Mathf.Clamp01(chargeTime / maxChargeTime);
        float finalSpeed = arrowSpeed * Mathf.Lerp(0.5f, 1f, chargePercent);

        Vector3 targetPoint = GetAimPoint();
        Vector3 launchDir = (targetPoint - firePoint.position).normalized;

        if (rb != null) rb.velocity = launchDir * finalSpeed;

        arrow.transform.rotation = Quaternion.LookRotation(launchDir, Vector3.up);

        Arrow arrowComp = arrow.GetComponent<Arrow>();
        if (arrowComp != null) arrowComp.SetArrowType(combat.currentArrowType);

        if (debugMode) Debug.Log($"Fired {combat.currentArrowType} arrow, charge {chargePercent:P0}, speed {finalSpeed}");
    }

    private Vector3 GetAimPoint()
    {
        Camera cam = move.GetActualCam();
        if (cam == null) return firePoint.position + firePoint.forward * aimMaxDistance;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance, aimLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (debugMode) Debug.Log($"Aim hit: {hit.collider.name} at {hit.point}");
            return hit.point;
        }

        return ray.origin + ray.direction * aimMaxDistance;
    }

    private void OnDrawGizmos()
    {
        if (!debugMode || firePoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(firePoint.position, GetAimPoint());
    }
}
