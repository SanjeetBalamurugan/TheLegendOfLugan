using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour
{
    [Header("Splash Settings")]
    [SerializeField] private GameObject splashPrefab;
    [SerializeField] private float splashDuration = 2f;
    [SerializeField] private string splashChimeKey = "Splash_Chime";

    [Header("Main Menu Settings")]
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private float menuFadeDuration = 1f;
    [SerializeField] private string mainMenuBGMKey = "MainMenuTheme";

    [Header("LevelSelectorUI")]
    [SerializeField] private GameObject levelSelectorUI;
    [SerializeField] private Button backButton;

    [Header("Fade Settings")]
    [SerializeField] private bool useAnim = true;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup splashCanvas;
    private CanvasGroup menuCanvas;
    private CanvasGroup levelSelectorUICanvas;

    private bool canvasLoaded = false;
    private bool handleUIInput = true;

    private void Start()
    {
        if (levelSelectorUI != null)
        {
            levelSelectorUICanvas = levelSelectorUI.GetComponent<CanvasGroup>();
            if (levelSelectorUICanvas == null)
                levelSelectorUICanvas = levelSelectorUI.AddComponent<CanvasGroup>();
            levelSelectorUI.SetActive(false);
            levelSelectorUICanvas.alpha = 0f;
        }

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainMenu);

        StartCoroutine(ShowSplashThenMenu());
    }

    private void Update()
    {
        if (canvasLoaded && handleUIInput) HandleUIInput();
    }

    private void HandleUIInput()
    {
        if(Input.anyKey)
        {
            if (useAnim)
                StartCoroutine(FadeCanvasGroup(levelSelectorUICanvas, 0f, 1f, fadeDuration));
            else
                levelSelectorUI.SetActive(true);
            handleUIInput = false;
            Debug.Log("KeyPressed");
        }
    }

    private IEnumerator ShowSplashThenMenu()
    {
        GameObject splash = Instantiate(splashPrefab, transform);
        splashCanvas = splash.GetComponent<CanvasGroup>();
        if (splashCanvas == null)
            splashCanvas = splash.AddComponent<CanvasGroup>();

        AudioManager.Instance.PlayUI(splashChimeKey);

        splashCanvas.alpha = 0f;
        float t = 0f;
        while (t < menuFadeDuration)
        {
            t += Time.deltaTime;
            splashCanvas.alpha = Mathf.Lerp(0f, 1f, t / menuFadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(splashDuration);

        t = 0f;
        while (t < menuFadeDuration)
        {
            t += Time.deltaTime;
            splashCanvas.alpha = Mathf.Lerp(1f, 0f, t / menuFadeDuration);
            yield return null;
        }

        Destroy(splash);

        GameObject menu = Instantiate(mainMenuPrefab, transform);
        menuCanvas = menu.GetComponent<CanvasGroup>();
        if (menuCanvas == null)
            menuCanvas = menu.AddComponent<CanvasGroup>();

        menuCanvas.alpha = 0f;
        t = 0f;
        while (t < menuFadeDuration)
        {
            t += Time.deltaTime;
            menuCanvas.alpha = Mathf.Lerp(0f, 1f, t / menuFadeDuration);
            yield return null;
        }

        AudioManager.Instance.PlayBGM(mainMenuBGMKey, 1f);
        canvasLoaded = true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration, System.Action onComplete = null)
    {
        if (cg == null) yield break;

        cg.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
        onComplete?.Invoke();
    }

    private IEnumerator FadeCanvasGroupOP(CanvasGroup cg, float start, float end, float duration, System.Action onComplete = null)
    {
        if (cg == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }
        cg.alpha = end;
        cg.gameObject.SetActive(false);

        onComplete?.Invoke();
    }

    public void BackToMainMenu()
    {
        if (useAnim)
            StartCoroutine(FadeCanvasGroupOP(levelSelectorUICanvas, 1f, 0f, fadeDuration));
        else
            levelSelectorUI.SetActive(true);
    }

    public void SetUIInput(bool _val)
    {
        handleUIInput = _val;
    }
}
