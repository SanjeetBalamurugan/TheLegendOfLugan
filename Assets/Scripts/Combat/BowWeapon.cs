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
    [SerializeField] private bool useChargeSystem = true; 

    [Header("Aiming")]
    [SerializeField] private float aimMaxDistance = 1000f;
    [SerializeField] private LayerMask aimLayerMask = ~0;

    private bool isAiming;
    private bool isCharging;
    private float chargeTime;

    private Vector3 lastAimPoint; // for debug drawing

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (isAiming)
        {
            if (useChargeSystem)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isCharging = true;
                    chargeTime = 0f;
                }

                if (isCharging)
                {
                    chargeTime += Time.deltaTime;
                    chargeTime = Mathf.Min(chargeTime, maxChargeTime);
                }

                if (Input.GetMouseButtonUp(0) && isCharging)
                {
                    TryFireArrow();
                    isCharging = false;
                    chargeTime = 0f;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    TryFireArrow();
                }
            }
        }
        else
        {
            isCharging = false;
            chargeTime = 0f;
        }

        // Update aim point every frame for debugging
        lastAimPoint = GetAimPoint();
    }

    private void TryFireArrow()
    {
        if (!combat.HasArrows())
        {
            Debug.Log("No " + combat.currentArrowType + " arrows left!");
            return;
        }

        if (combat.TryConsumeArrow())
        {
            FireArrow();
        }
        else
        {
            Debug.Log("Failed to consume arrow: " + combat.currentArrowType);
        }
    }

    private void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        float finalSpeed = arrowSpeed;

        if (useChargeSystem && maxChargeTime > 0f)
        {
            float chargePercent = Mathf.Clamp01(chargeTime / maxChargeTime);
            finalSpeed *= Mathf.Lerp(0.5f, 1f, chargePercent); // slow at low charge, full at max
        }

        Vector3 launchDir = (lastAimPoint - firePoint.position).normalized;

        if (rb != null)
        {
            rb.velocity = launchDir * finalSpeed;
        }

        arrow.transform.rotation = Quaternion.LookRotation(launchDir, Vector3.up);

        Arrow arrowComp = arrow.GetComponent<Arrow>();
        if (arrowComp != null)
        {
            arrowComp.SetArrowType(combat.currentArrowType);
        }

        Debug.Log($"Fired {combat.currentArrowType} arrow at {lastAimPoint} with speed {finalSpeed}");
    }

    private Vector3 GetAimPoint()
    {
        Camera cam = move.GetActualCam();
        if (cam == null)
        {
            return firePoint.position + firePoint.forward * aimMaxDistance;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance, aimLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red, 0.01f);
            return hit.point;
        }

        Debug.DrawRay(ray.origin, ray.direction * aimMaxDistance, Color.green, 0.01f);
        return ray.origin + ray.direction * aimMaxDistance;
    }

    // --- Debug Gizmos ---
    private void OnDrawGizmos()
    {
        if (firePoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(firePoint.position, 0.05f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(firePoint.position, lastAimPoint);
        Gizmos.DrawSphere(lastAimPoint, 0.05f);
    }
}
