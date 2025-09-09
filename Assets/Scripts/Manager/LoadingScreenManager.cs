using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;         // The masked/fillable image
    [SerializeField] private Text percentageText;     // The percentage text

    private void OnEnable()
    {
        // Subscribe to scene loading
        GameSceneManager.OnSceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        GameSceneManager.OnSceneLoaded -= HandleSceneLoaded;
    }

    /// <summary>
    /// Starts the loading bar process when GameSceneManager begins loading.
    /// </summary>
    public void StartLoading(GameScene sceneToLoad)
    {
        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    private IEnumerator LoadSceneAsync(GameScene scene)
    {
        // Reset UI
        if (fillImage != null) fillImage.fillAmount = 0f;
        if (percentageText != null) percentageText.text = "0%";

        // Begin async loading
        AsyncOperation op = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        op.allowSceneActivation = false; // Control when the scene is activated

        while (!op.isDone)
        {
            // Progress is capped at 0.9f until scene is ready
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (fillImage != null)
                fillImage.fillAmount = progress;

            if (percentageText != null)
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";

            // If fully loaded, activate scene
            if (progress >= 1f)
                op.allowSceneActivation = true;

            yield return null;
        }
    }

    private void HandleSceneLoaded(GameScene scene)
    {
        // Optional: Hide or disable the loading UI when the new scene is ready
        gameObject.SetActive(false);
    }
}
