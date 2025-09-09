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
        
        GameSceneManager.OnSceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        
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

        
        AsyncOperation op = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        op.allowSceneActivation = false; 

        while (!op.isDone)
        {
            
            float progress = Mathf.Clamp01(op.progress / 0.9f);

            if (fillImage != null)
                fillImage.fillAmount = progress;

            if (percentageText != null)
                percentageText.text = Mathf.RoundToInt(progress * 100f) + "%";

            
            if (progress >= 1f)
                op.allowSceneActivation = true;

            yield return null;
        }
    }

    private void HandleSceneLoaded(GameScene scene)
    {
        
        gameObject.SetActive(false);
    }
}
