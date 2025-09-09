using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Interactive Object";
    [SerializeField] private Transform finalPlacement;
    [SerializeField] private List<GameObject> platformObjects;
    [SerializeField] private float moveDuration = 1f;

    private Dictionary<GameObject, Coroutine> activeMoves = new();

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (GameObject platform in platformObjects)
        {
            if (activeMoves.ContainsKey(platform) && activeMoves[platform] != null)
                StopCoroutine(activeMoves[platform]);

            activeMoves[platform] = StartCoroutine(MovePlatform(platform));
        }
    }

    private IEnumerator MovePlatform(GameObject platform)
    {
        Vector3 startPos = platform.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, finalPlacement.position.y, startPos.z);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            t = Mathf.SmoothStep(0f, 1f, t); // ease in-out
            platform.transform.position = Vector3.Lerp(startPos, targetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        platform.transform.position = targetPos;
        activeMoves[platform] = null;
    }
}
