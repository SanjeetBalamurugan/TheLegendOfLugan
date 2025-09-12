using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChromaPlatformButton : MonoBehaviour, IArrowInteractable
{
    [SerializeField] private string objectName = "Chroma Platform";
    [SerializeField] private List<Renderer> platformRenderers;
    [SerializeField] private string alphaThresholdProperty = "_AlphaThreshold";
    [SerializeField] private float targetAlphaThreshold = 1f;
    [SerializeField] private float transitionDuration = 1f;

    private Dictionary<Renderer, Coroutine> activeTransitions = new();
    private Dictionary<Renderer, Material> cachedMaterials = new();

    private void Awake()
    {
        foreach (var rend in platformRenderers)
        {
            if (rend == null) continue;
            cachedMaterials[rend] = rend.material;
            rend.gameObject.GetComponent<Collider>().enabled = false;
        }
    }

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (Renderer rend in platformRenderers)
        {
            if (rend == null) continue;
            rend.gameObject.GetComponent<Collider>().enabled = true;

            if (activeTransitions.ContainsKey(rend) && activeTransitions[rend] != null)
                StopCoroutine(activeTransitions[rend]);

            activeTransitions[rend] = StartCoroutine(ChangeAlphaThreshold(rend));
        }
    }

    private IEnumerator ChangeAlphaThreshold(Renderer rend)
    {
        Material mat = cachedMaterials[rend];

        if (!mat.HasProperty(alphaThresholdProperty))
        {
            Debug.LogWarning($"[ChromaPlatformButton] Material on '{rend.gameObject.name}' lacks property '{alphaThresholdProperty}'.");
            yield break;
        }

        float startValue = mat.GetFloat(alphaThresholdProperty);
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / transitionDuration);
            float newValue = Mathf.Lerp(startValue, targetAlphaThreshold, t);

            mat.SetFloat(alphaThresholdProperty, newValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mat.SetFloat(alphaThresholdProperty, targetAlphaThreshold);
        activeTransitions.Remove(rend);
    }
}
