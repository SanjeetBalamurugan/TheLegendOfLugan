using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPrefab;
    [SerializeField] private GameObject settingsMenuPrefab;
    [SerializeField] private Image elementIcon;
    [SerializeField] private Text elementNameText;
    [SerializeField] private TPVPlayerCombat playerCombat;

    [Header("Optional Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button backButton;

    [Header("Fade Settings")]
    [SerializeField] private bool useAnim = true;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("Element Sprites")]
    [SerializeField] private Sprite physicalSprite;
    [SerializeField] private Sprite pyroSprite;
    [SerializeField] private Sprite hydroSprite;
    [SerializeField] private Sprite defaultElementSprite;
    [SerializeField] private string defaultElementName = "None";

    private CanvasGroup pauseCanvasGroup;
    private bool isPaused = false;
    private TPVPlayerCombat.ArrowType lastArrowType;

    private void Awake()
    {
        if (playerCombat == null)
            playerCombat = FindObjectOfType<TPVPlayerCombat>();

        SetElementUI(defaultElementSprite, defaultElementName);
    }

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

        UpdateElementUI(playerCombat.GetArrowType());
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

        if (playerCombat != null)
        {
            var currentArrow = playerCombat.GetArrowType();
            if (currentArrow != lastArrowType)
            {
                UpdateElementUI(currentArrow);
                lastArrowType = currentArrow;
            }
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

        if (elementIcon != null) elementIcon.gameObject.SetActive(false);
        if (elementNameText != null) elementNameText.gameObject.SetActive(false);

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

        if (elementIcon != null) elementIcon.gameObject.SetActive(true);
        if (elementNameText != null) elementNameText.gameObject.SetActive(true);

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

    public void UpdateElementUI(TPVPlayerCombat.ArrowType arrowType)
    {
        Sprite elementSprite = defaultElementSprite;
        string elementName = defaultElementName;

        switch (arrowType)
        {
            case TPVPlayerCombat.ArrowType.Physical:
                elementSprite = physicalSprite;
                elementName = "Physical";
                break;
            case TPVPlayerCombat.ArrowType.Pyro:
                elementSprite = pyroSprite;
                elementName = "Pyro";
                break;
            case TPVPlayerCombat.ArrowType.Hydro:
                elementSprite = hydroSprite;
                elementName = "Hydro";
                break;
        }

        SetElementUI(elementSprite, elementName);
    }

    private void SetElementUI(Sprite sprite, string name)
    {
        if (elementIcon != null) elementIcon.sprite = sprite;
        if (elementNameText != null) elementNameText.text = name;
    }
}
