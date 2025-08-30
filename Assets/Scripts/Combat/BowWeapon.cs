using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private TPVPlayerCombat combat;

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
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        float finalSpeed = arrowSpeed;

        if (useChargeSystem && maxChargeTime > 0f)
        {
            float chargePercent = Mathf.Clamp01(chargeTime / maxChargeTime);
            finalSpeed *= Mathf.Lerp(0.5f, 1f, chargePercent); // slow at low charge, full at max
        }

        Vector3 targetPoint = GetAimPoint();
        Vector3 launchDir = (targetPoint - firePoint.position).normalized;

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

        Debug.Log("Fired " + combat.currentArrowType + " arrow with speed " + finalSpeed);
    }

    private Vector3 GetAimPoint()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            return firePoint.position + firePoint.forward * aimMaxDistance;
        }

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance, aimLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point;
        }

        return ray.origin + ray.direction * aimMaxDistance;
    }
}
