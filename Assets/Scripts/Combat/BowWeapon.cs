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

    private bool isAiming;
    private bool isCharging;
    private float chargeTime;

    private void Update()
    {
        isAiming = Input.GetMouseButton(1);

        if (isAiming)
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

        float chargePercent = chargeTime / maxChargeTime;
        float finalSpeed = arrowSpeed;

        if (rb != null)
        {
            rb.velocity = firePoint.forward * finalSpeed;
        }

        Arrow arrowComp = arrow.GetComponent<Arrow>();
        if (arrowComp != null)
        {
            Debug.Log("Abca");
            arrowComp.SetArrowType(combat.currentArrowType);
        }

        Debug.Log("Fired " + combat.currentArrowType + " arrow with speed " + finalSpeed);
    }
}
