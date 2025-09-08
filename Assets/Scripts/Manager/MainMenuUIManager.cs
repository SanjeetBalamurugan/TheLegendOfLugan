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

    private CanvasGroup splashCanvas;
    private CanvasGroup menuCanvas;

    private void Start()
    {
        StartCoroutine(ShowSplashThenMenu());
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
    }
}
