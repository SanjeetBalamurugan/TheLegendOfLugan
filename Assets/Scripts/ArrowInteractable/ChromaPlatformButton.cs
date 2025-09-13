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
    [SerializeField] private GameObject explosionPrefab;

    private Dictionary<Renderer, Coroutine> activeTransitions = new();
    private Dictionary<Renderer, Material> cachedMaterials = new();
    private Dictionary<Renderer, Collider> cachedColliders = new();

    private void Awake()
    {
        foreach (var rend in platformRenderers)
        {
            if (rend == null) continue;
            cachedMaterials[rend] = rend.material;
            var collider = rend.gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                cachedColliders[rend] = collider;
                collider.enabled = false;
            }
        }
    }

    public void OnArrowHit(TPVPlayerCombat.ArrowType arrowType)
    {
        foreach (Renderer rend in platformRenderers)
        {
            if (rend == null) continue;

            Material mat = cachedMaterials[rend];
            if (mat.HasProperty(alphaThresholdProperty))
            {
                mat.SetFloat(alphaThresholdProperty, 1f);
            }

            if (cachedColliders.ContainsKey(rend))
            {
                cachedColliders[rend].enabled = true;
            }

            if (activeTransitions.ContainsKey(rend) && activeTransitions[rend] != null)
            {
                StopCoroutine(activeTransitions[rend]);
            }

            activeTransitions[rend] = StartCoroutine(ChangeAlphaThreshold(rend));
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        StartCoroutine(DestroyPlatformWithDelay());
    }

    private IEnumerator ChangeAlphaThreshold(Renderer rend)
    {
        Material mat = cachedMaterials[rend];

        if (!mat.HasProperty(alphaThresholdProperty))
        {
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

    private IEnumerator DestroyPlatformWithDelay()
    {
        yield return new WaitForSeconds(transitionDuration);
        Destroy(gameObject);
    }
}
