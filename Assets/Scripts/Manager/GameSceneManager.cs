using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameScene
{
    None,          // added to avoid default 0 bug
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
    [SerializeField] private GameScene currentScene = GameScene.None;

    public static event System.Action<GameScene> OnSceneLoaded;

    private void Start()
    {
        // Load settings automatically at startup
        LoadSettingsData();
    }

    private IEnumerator LoadGameSequence()
    {
        // Ensure Persistent is always loaded
        if (!SceneManager.GetSceneByName(GameScene.Persistent.ToString()).isLoaded)
            yield return SceneManager.LoadSceneAsync(GameScene.Persistent.ToString(), LoadSceneMode.Additive);

        if (useLoadingScreen)
        {
            // Load the loading screen
            yield return SceneManager.LoadSceneAsync(GameScene.LoadingScreen.ToString(), LoadSceneMode.Additive);

            // Load the actual scene
            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));

            // Unload the loading screen
            yield return SceneManager.UnloadSceneAsync(GameScene.LoadingScreen.ToString());
        }
        else
        {
            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));
        }
    }

    private IEnumerator LoadNextSceneAsync(GameScene scene)
    {
        // Don’t unload if first load or if last scene was LoadingScreen
        if (currentScene != GameScene.None && currentScene != GameScene.LoadingScreen)
            yield return SceneManager.UnloadSceneAsync(currentScene.ToString());

        // Start loading the new scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Set it as the active scene
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));

        // Update tracker
        currentScene = scene;

        // Notify listeners
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
        return currentScene;
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
