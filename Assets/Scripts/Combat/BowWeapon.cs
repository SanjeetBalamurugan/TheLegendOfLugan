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

    private bool isAiming;
    private float chargeTime;
    private GameObject chargingArrow;

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (isAiming)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxChargeTime);

            if (chargingArrow == null)
            {
                chargingArrow = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation, firePoint);
                Rigidbody rb = chargingArrow.GetComponent<Rigidbody>();
                if (rb) rb.isKinematic = true;
                Collider col = chargingArrow.GetComponent<Collider>();
                if (col) col.enabled = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                TryFireArrow();
            }
        }
        else
        {
            chargeTime = 0f;
            if (chargingArrow) Destroy(chargingArrow);
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
    }

    private void FireArrow()
    {
        if (chargingArrow == null) return;

        chargingArrow.transform.parent = null;
        Rigidbody rb = chargingArrow.GetComponent<Rigidbody>();
        Collider col = chargingArrow.GetComponent<Collider>();

        float finalSpeed = arrowSpeed * Mathf.Clamp01(chargeTime / maxChargeTime);

        if (rb)
        {
            rb.isKinematic = false;
            rb.velocity = (GetAimPoint() - firePoint.position).normalized * finalSpeed;
        }

        if (col) col.enabled = true;

        Arrow arrowComp = chargingArrow.GetComponent<Arrow>();
        if (arrowComp != null)
            arrowComp.SetArrowType(combat.currentArrowType);

        chargingArrow.transform.rotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
        chargingArrow = null;
        chargeTime = 0f;
    }

    private Vector3 GetAimPoint()
    {
        Camera cam = move.GetActualCam();
        if (cam == null) return firePoint.position + firePoint.forward * aimMaxDistance;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance, aimLayerMask, QueryTriggerInteraction.Ignore))
            return hit.point;

        return ray.origin + ray.direction * aimMaxDistance;
    }
}
