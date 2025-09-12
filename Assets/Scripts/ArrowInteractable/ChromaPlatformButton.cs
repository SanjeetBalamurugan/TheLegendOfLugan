using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChromaPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Chroma Platform";
    [SerializeField] private List<Renderer> platformRenderers;
    [SerializeField] private string alphaThresholdProperty = "_AlphaThreshold";
    [SerializeField] private float targetAlphaThreshold = 0.5f;
    [SerializeField] private float transitionDuration = 1f;

    private Dictionary<Renderer, Coroutine> activeTransitions = new();

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (Renderer rend in platformRenderers)
        {
            if (rend == null)
            {
                Debug.Log(rend.gameObject.name);
                continue;
            }

            if (activeTransitions.ContainsKey(rend) && activeTransitions[rend] != null)
                StopCoroutine(activeTransitions[rend]);

            activeTransitions[rend] = StartCoroutine(ChangeAlphaThreshold(rend));
        }
    }

    private IEnumerator ChangeAlphaThreshold(Renderer rend)
    {
        if (!rend.material.HasProperty(alphaThresholdProperty))
        {
            Debug.Log("{rend.gameObject.name} doesnt have the alpha property");
            yield break;
        }
            

        float startValue = rend.material.GetFloat(alphaThresholdProperty);
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            float newValue = Mathf.Lerp(startValue, targetAlphaThreshold, t);
            rend.material.SetFloat(alphaThresholdProperty, newValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rend.material.SetFloat(alphaThresholdProperty, targetAlphaThreshold);
        activeTransitions[rend] = null;
    }
}
