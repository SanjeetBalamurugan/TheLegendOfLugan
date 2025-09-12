using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameScene
{
    Persistent,
    MainMenu,
    Gameplay,
    LoadingScreen,
    Level1,
    Level2
}

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private bool useLoadingScreen = true;
    [SerializeField] private GameScene nextSceneToLoad;

    public static event System.Action<GameScene> OnSceneLoaded;

    private void Start()
    {
        // Load settings automatically at startup
        LoadSettingsData();
        StartCoroutine(LoadGameSequence());
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
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        SceneManager.UnloadSceneAsync(nextSceneToLoad.ToString());
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));

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
