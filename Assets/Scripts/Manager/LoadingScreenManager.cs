using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressFill;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        canvasGroup.alpha = 0f;
    }

    public void SetProgress(float value)
    {
        if (progressFill != null)
            progressFill.fillAmount = Mathf.Clamp01(value);
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(0f));
    }

    private IEnumerator FadeCanvas(float target)
    {
        float start = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }
}
