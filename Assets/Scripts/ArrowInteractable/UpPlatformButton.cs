using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Interactive Object";
    [SerializeField] private Transform finalPlacement;
    [SerializeField] private List<GameObject> platformObjects;
    [SerializeField] private float moveSmoothness = 1f;
    [SerializeField] private GameObject explosionPrefab;

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (GameObject platform in platformObjects)
        {
            StartCoroutine(MovePlatform(platform));
        }
    }

    private IEnumerator MovePlatform(GameObject platform)
    {
        Vector3 startPos = platform.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, finalPlacement.position.y, startPos.z);

        float elapsed = 0f;
        while (elapsed < moveSmoothness)
        {
            platform.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveSmoothness);
            elapsed += Time.deltaTime;
            yield return null;
        }

        platform.transform.position = targetPos;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, platform.transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Destroy the GameObject this script is attached to
    }
}
