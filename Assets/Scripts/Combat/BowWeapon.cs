using UnityEngine;

public class BowWeapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject physicalArrowPrefab;
    [SerializeField] private GameObject pyroArrowPrefab;
    [SerializeField] private GameObject hydroArrowPrefab;
    [SerializeField] private TPVPlayerMove player;
    [SerializeField] private GameObject chargePlane;
    [SerializeField] private string emissionProperty = "_EmissionFactor";

    [Header("Bow Settings")]
    public float maxChargeTime = 2f;
    public float minShootForce = 10f;
    public float maxShootForce = 40f;

    private float currentChargeTime;
    private bool isCharging;
    private bool isFullyCharged;
    private GameObject currentArrow;
    private Material chargePlaneMat;

    void Start()
    {
        if (chargePlane != null)
        {
            chargePlane.SetActive(false);
            chargePlaneMat = chargePlane.GetComponent<Renderer>().material;
        }
    }

    public void HandleCombat(TPVPlayerCombat.ArrowType arrowType)
    {
        if (player != null && player.IsAiming)
        {
            if (!isCharging) StartCharging(arrowType);
            ChargeArrow();
        }
        else
        {
            if (isCharging) ReleaseArrow();
        }
    }

    private void StartCharging(TPVPlayerCombat.ArrowType arrowType)
    {
        isCharging = true;
        isFullyCharged = false;
        currentChargeTime = 0f;

        GameObject prefabToUse = GetArrowPrefab(arrowType);
        currentArrow = Instantiate(prefabToUse, arrowSpawnPoint.position, arrowSpawnPoint.rotation, arrowSpawnPoint);
        currentArrow.GetComponent<Rigidbody>().isKinematic = true;
        currentArrow.transform.SetParent(arrowSpawnPoint);

        if (chargePlane != null) chargePlane.SetActive(true);
    }

    private GameObject GetArrowPrefab(TPVPlayerCombat.ArrowType type)
    {
        switch (type)
        {
            case TPVPlayerCombat.ArrowType.Pyro:
                return pyroArrowPrefab;
            case TPVPlayerCombat.ArrowType.Hydro:
                return hydroArrowPrefab;
            case TPVPlayerCombat.ArrowType.Physical:
            default:
                return physicalArrowPrefab;
        }
    }

    private void ChargeArrow()
    {
        if (!isFullyCharged)
        {
            currentChargeTime += Time.deltaTime;
            if (currentChargeTime >= maxChargeTime)
            {
                currentChargeTime = maxChargeTime;
                isFullyCharged = true;
            }
        }

        if (chargePlaneMat != null)
        {
            float chargePercent = currentChargeTime / maxChargeTime;
            chargePlaneMat.SetFloat(emissionProperty, chargePercent);
        }
    }

    private void ReleaseArrow()
    {
        isCharging = false;

        float chargePercent = currentChargeTime / maxChargeTime;
        float shootForce = Mathf.Lerp(minShootForce, maxShootForce, chargePercent);

        currentArrow.transform.SetParent(null);
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(arrowSpawnPoint.forward * shootForce, ForceMode.Impulse);

        if (chargePlane != null) chargePlane.SetActive(false);
        if (chargePlaneMat != null) chargePlaneMat.SetFloat(emissionProperty, 0f);

        currentArrow = null;
    }
}
