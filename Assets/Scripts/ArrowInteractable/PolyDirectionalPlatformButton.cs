using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolyDirectionalPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "PolyDirectional Platform";
    [SerializeField] private Transform finalPlacement;
    [SerializeField] private List<GameObject> platformObjects;
    [SerializeField] private float moveDuration = 1f;

    [Header("Axis Movement Control")]
    [SerializeField] private bool moveX = false;
    [SerializeField] private bool moveY = true;
    [SerializeField] private bool moveZ = false;
    [SerializeField] private TPVPlayerCombat.ArrowType trigger = TPVPlayerCombat.ArrowType.Physical;
    [SerializeField] private GameObject explosion;

    private bool isHit = false;
    private Dictionary<GameObject, Coroutine> activeMoves = new();

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (GameObject platform in platformObjects)
        {
            if (activeMoves.ContainsKey(platform) && activeMoves[platform] != null && trigger == arrowType)
            {
                StopCoroutine(activeMoves[platform]);
                Instantiate(explosion, this.transform);
                Destroy(this.gameObject);
            }

            activeMoves[platform] = StartCoroutine(MovePlatform(platform));
        }
    }

    private IEnumerator MovePlatform(GameObject platform)
    {
        Vector3 startPos = platform.transform.position;
        Vector3 targetPos = new Vector3(
            moveX ? finalPlacement.position.x : startPos.x,
            moveY ? finalPlacement.position.y : startPos.y,
            moveZ ? finalPlacement.position.z : startPos.z
        );

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
