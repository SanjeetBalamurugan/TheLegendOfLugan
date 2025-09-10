using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    public static LoadingScreenManager Instance;
    
    [SerializeField] private GameObject loadingUI;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image progressFill;
    [SerializeField] private Text progressText;
    [SerializeField] private float fadeDuration = 0.5f;

    public bool IsFullyVisible { get; private set; }
    public bool IsFullyHidden { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        canvasGroup = loadingUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = loadingUI.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        IsFullyHidden = true;
        IsFullyVisible = false;
    }

    public void SetProgress(float value)
    {
        float percent = Mathf.Clamp01(value) * 100f;
        if (progressFill != null) progressFill.fillAmount = value;
        if (progressText != null) progressText.text = $"{percent:0}%";
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
        IsFullyHidden = false;
        IsFullyVisible = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = target;

        IsFullyVisible = target == 1f;
        IsFullyHidden = target == 0f;
    }
}
