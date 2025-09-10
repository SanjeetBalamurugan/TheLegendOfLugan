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
    [SerializeField] private GameObject pressAnyKeyPrefab;
    [SerializeField] private Animator mainMenuAnimator; // assign animator that has GoToPortal

    [Header("Level Selector Settings")]
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
    private Coroutine blinkRoutine;

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

        if (pressAnyKeyPrefab != null)
            pressAnyKeyPrefab.SetActive(false);

        StartCoroutine(ShowSplashThenMenu());
    }

    private void Update()
    {
        if (canvasLoaded && handleUIInput)
            HandleUIInput();
    }

    private void HandleUIInput()
    {
        if (Input.anyKeyDown)
        {
            // Stop blinking text
            if (pressAnyKeyPrefab != null)
            {
                pressAnyKeyPrefab.SetActive(false);
                if (blinkRoutine != null) StopCoroutine(blinkRoutine);
            }

            // Fade out main menu
            if (menuCanvas != null && useAnim)
                StartCoroutine(FadeCanvasGroup(menuCanvas, 1f, 0f, fadeDuration));

            // Play transition animation
            if (mainMenuAnimator != null)
                mainMenuAnimator.SetTrigger("GoToPortal");
            else
                ShowLevelSelector(); // fallback if animator not set

            handleUIInput = false;
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

        // Show blinking "Press Any Key"
        if (pressAnyKeyPrefab != null)
        {
            pressAnyKeyPrefab.SetActive(true);
            blinkRoutine = StartCoroutine(BlinkText(pressAnyKeyPrefab));
        }

        canvasLoaded = true;
    }

    private IEnumerator BlinkText(GameObject textObj)
    {
        CanvasGroup cg = textObj.GetComponent<CanvasGroup>();
        if (cg == null) cg = textObj.AddComponent<CanvasGroup>();

        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);

            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
        }
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
            levelSelectorUI.SetActive(false);
    }

    /// <summary>
    /// Called by Animation Event at the end of "GoToPortal"
    /// </summary>
    public void OnGoToPortalAnimationEnd()
    {
        ShowLevelSelector();
    }

    private void ShowLevelSelector()
    {
        if (useAnim)
            StartCoroutine(FadeCanvasGroup(levelSelectorUICanvas, 0f, 1f, fadeDuration));
        else
            levelSelectorUI.SetActive(true);
    }

    public void SetUIInput(bool _val)
    {
        handleUIInput = _val;
    }
}
