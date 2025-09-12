using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameScene
{
    Persistent,
    MainMenu,
    LoadingScreen,
    Level1,
    Level2
}

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private bool useLoadingScreen = true;
    private GameScene nextSceneToLoad;
    [SerializeField] private GameScene currentScene;

    public static event System.Action<GameScene> OnSceneLoaded;

    private void Start()
    {
        // Load settings automatically at startup
        LoadSettingsData();
    }

    private IEnumerator LoadGameSequence()
    {
        if (!SceneManager.GetSceneByName(GameScene.Persistent.ToString()).isLoaded)
            yield return SceneManager.LoadSceneAsync(GameScene.Persistent.ToString(), LoadSceneMode.Additive);

        if (useLoadingScreen)
        {
            yield return SceneManager.LoadSceneAsync(GameScene.LoadingScreen.ToString(), LoadSceneMode.Additive);
            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));
            SceneManager.UnloadSceneAsync(GameScene.LoadingScreen.ToString());
        }
        else
        {
            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));
        }
    }

    private IEnumerator LoadNextSceneAsync(GameScene scene)
    {
        if (SceneManager.GetSceneByName(currentScene.ToString()).isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene.ToString());
            if (unloadOp != null)
                yield return unloadOp;
        }

        if (!Application.CanStreamedLevelBeLoaded(scene.ToString()))
            yield break;

        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.FadeIn();

>>>>>>> parent of e4b46c7 (Update GameSceneManager.cs)
        SceneManager.UnloadSceneAsync(currentScene.ToString());
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            if (LoadingScreenManager.Instance != null)
                LoadingScreenManager.Instance.SetProgress(loadOp.progress);
            yield return null;
        }

        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.SetProgress(1f);

        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        currentScene = scene;

        yield return null;

        if (LoadingScreenManager.Instance != null)
            LoadingScreenManager.Instance.FadeOut();
=======
>>>>>>> parent of e4b46c7 (Update GameSceneManager.cs)

        OnSceneLoaded?.Invoke(scene);
    }

    public void LoadScene(GameScene scene, bool useLoadingScreenOverride = true)
    {
        nextSceneToLoad = scene;
        useLoadingScreen = useLoadingScreenOverride;
        StartCoroutine(LoadGameSequence());
    }

    public void ReloadCurrentScene()
    {
        GameScene currentScene = (GameScene)System.Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name);
        LoadScene(currentScene, useLoadingScreen);
    }

    public void QuitToMainMenu()
    {
        // Save settings before quitting
        SaveSettingsData();
        LoadScene(GameScene.MainMenu, true);
    }

    public GameScene GetCurrentScene()
    {
        return (GameScene)System.Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name);
    }

    // --- SAVE/LOAD HOOKS ---
    private void SaveSettingsData()
    {
        // Example: pull data from AudioManager
        float bgm = PlayerPrefs.GetFloat("BGMVol", 1f);
        float ui = PlayerPrefs.GetFloat("UIVol", 1f);
        int quality = QualitySettings.GetQualityLevel();

        var data = new SettingsSaveData(bgm, ui, quality);
        SaveSystem.Save(SaveType.Settings, data);
    }

    private void LoadSettingsData()
    {
        SettingsSaveData data = SaveSystem.Load<SettingsSaveData>(SaveType.Settings);
        if (data != null)
        {
            // Apply audio
            AudioManager.Instance.SetBGMVolume(data.bgmVolume);
            AudioManager.Instance.SetUIVolume(data.uiVolume);

            // Apply graphics
            QualitySettings.SetQualityLevel(data.qualityPreset, true);
        }
    }
}
