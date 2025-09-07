using UnityEngine;
using System.Collections;

public class ElementalTargetChain : MonoBehaviour, IArrowInteractable
{
    [Header("Sequence Settings")]
    [SerializeField] private TPVPlayerCombat.ArrowType requiredArrowType;
    [SerializeField] private ElementalTargetChain nextTarget;

    [Header("Light Settings")]
    [SerializeField] private Light pointLight;
    [SerializeField] private float blinkIntensityMin = 1f;
    [SerializeField] private float blinkIntensityMax = 3f;
    [SerializeField] private float blinkSpeed = 2f;

    [Header("Sequence Completion")]
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private Transform finalPlacement;
    [SerializeField] private float moveDuration = 2f;

    [Header("Wrong Sequence Feedback")]
    [SerializeField] private Color wrongColor = Color.red;
    [SerializeField] private float wrongBlinkIntensity = 5f;
    [SerializeField] private float wrongBlinkSpeed = 5f;

    private bool isActivated = false;
    private Coroutine blinkCoroutine;

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        if (isActivated) return;

        if (arrowType == requiredArrowType)
        {
            ActivateTarget();

            if (nextTarget != null)
            {
                nextTarget.gameObject.SetActive(true);
            }
            else
            {
                if (objectToMove != null && finalPlacement != null)
                {
                    StartCoroutine(MoveObjectToPosition(objectToMove, finalPlacement.position.y, moveDuration));
                }
            }
        }
        else
        {
            StartCoroutine(WrongSequenceFeedback());
        }
    }

    private void ActivateTarget()
    {
        isActivated = true;
        if (pointLight != null)
        {
            pointLight.enabled = true;
            blinkCoroutine = StartCoroutine(BlinkLight());
        }
    }

    private IEnumerator BlinkLight()
    {
        Color originalColor = pointLight.color;
        while (true)
        {
            float intensity = Mathf.Lerp(blinkIntensityMin, blinkIntensityMax, Mathf.PingPong(Time.time * blinkSpeed, 1f));
            pointLight.intensity = intensity;
            pointLight.color = originalColor;
            yield return null;
        }
    }

    private IEnumerator MoveObjectToPosition(GameObject obj, float targetY, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);

        while (elapsed < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPos;
    }

    private IEnumerator WrongSequenceFeedback()
    {
        StopAllCoroutines();
        isActivated = false;

        Light[] lightsInChain = GetComponentsInChildren<Light>();
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            float intensity = Mathf.Lerp(wrongBlinkIntensity, 0f, elapsed / duration);
            foreach (Light l in lightsInChain)
            {
                l.color = wrongColor;
                l.intensity = intensity;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (Light l in lightsInChain)
        {
            l.intensity = 0f;
        }

        if (nextTarget != null)
            nextTarget.ResetChain();
    }

    public void ResetChain()
    {
        isActivated = false;
        if (pointLight != null)
        {
            pointLight.enabled = false;
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
        }

        if (nextTarget != null)
            nextTarget.ResetChain();
    }
}
