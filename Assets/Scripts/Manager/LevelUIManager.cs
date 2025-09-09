using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject settingsMenuPrefab;

    [Header("Optional Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;

    [Header("Fade Settings")]
    [SerializeField] private bool useAnim = true;
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup pauseCanvasGroup;
    private bool isPaused = false;

    private void Start()
    {
        if (pauseMenuPrefab != null)
        {
            pauseCanvasGroup = pauseMenuPrefab.GetComponent<CanvasGroup>();
            if (pauseCanvasGroup == null)
                pauseCanvasGroup = pauseMenuPrefab.AddComponent<CanvasGroup>();
            pauseMenuPrefab.SetActive(false);
            pauseCanvasGroup.alpha = 0f;
        }

        if (settingsMenuPrefab != null)
            settingsMenuPrefab.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);

        if (backButton != null)
            backButton.onClick.AddListener(BackToPauseMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        if (pauseMenuPrefab == null) return;

        if (settingsMenuPrefab != null)
            settingsMenuPrefab.SetActive(false);

        if (useAnim)
            StartCoroutine(FadeCanvasGroup(pauseCanvasGroup, 0f, 1f, fadeDuration));
        else
            pauseMenuPrefab.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseMenuPrefab == null) return;

        if (useAnim)
            StartCoroutine(FadeCanvasGroup(pauseCanvasGroup, 1f, 0f, fadeDuration, () =>
            {
                pauseMenuPrefab.SetActive(false);
            }));
        else
            pauseMenuPrefab.SetActive(false);

        if (settingsMenuPrefab != null)
            settingsMenuPrefab.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OpenSettings()
    {
        if (settingsMenuPrefab == null) return;

        settingsMenuPrefab.SetActive(true);
        if (pauseMenuPrefab != null)
            pauseMenuPrefab.SetActive(false);
    }

    public void BackToPauseMenu()
    {
        if (pauseMenuPrefab == null) return;

        if (settingsMenuPrefab != null)
            settingsMenuPrefab.SetActive(false);

        if (useAnim)
            StartCoroutine(FadeCanvasGroup(pauseCanvasGroup, 0f, 1f, fadeDuration));
        else
            pauseMenuPrefab.SetActive(true);
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
}
