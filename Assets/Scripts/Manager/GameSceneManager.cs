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

    public static GameSceneManager Instance { get; private set; }
    public static event System.Action<GameScene> OnSceneLoaded;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadSettingsData();
    }

    private IEnumerator LoadGameSequence(bool useLoadingScreenOverride)
    {
        if (!SceneManager.GetSceneByName(GameScene.Persistent.ToString()).isLoaded)
        {
            if (Application.CanStreamedLevelBeLoaded(GameScene.Persistent.ToString()))
                yield return SceneManager.LoadSceneAsync(GameScene.Persistent.ToString(), LoadSceneMode.Additive);
            else
                yield break;
        }

        if (useLoadingScreenOverride)
        {
            if (Application.CanStreamedLevelBeLoaded(GameScene.LoadingScreen.ToString()))
                yield return SceneManager.LoadSceneAsync(GameScene.LoadingScreen.ToString(), LoadSceneMode.Additive);
        }

        yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));

        if (useLoadingScreenOverride && SceneManager.GetSceneByName(GameScene.LoadingScreen.ToString()).isLoaded)
        {
            AsyncOperation unloadLoadingOp = SceneManager.UnloadSceneAsync(GameScene.LoadingScreen.ToString());
            while (!unloadLoadingOp.isDone)
                yield return null;
        }
    }

    private IEnumerator LoadNextSceneAsync(GameScene scene)
    {
        string currentSceneName = currentScene.ToString();

        if (currentScene != GameScene.Persistent && SceneManager.GetSceneByName(currentSceneName).isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentSceneName);
            while (!unloadOp.isDone)
                yield return null;
        }

        if (!Application.CanStreamedLevelBeLoaded(scene.ToString()))
            yield break;

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        currentScene = scene;

        // ✅ Switch BGM based on scene
        PlaySceneBGM(scene);

        OnSceneLoaded?.Invoke(scene);
    }

    public void LoadScene(GameScene scene, bool useLoadingScreenOverride = true)
    {
        nextSceneToLoad = scene;
        useLoadingScreen = useLoadingScreenOverride;
        StartCoroutine(LoadGameSequence(useLoadingScreenOverride));
    }

    public void ReloadCurrentScene()
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out GameScene scene))
            LoadScene(scene, useLoadingScreen);
    }

    public void QuitToMainMenu()
    {
        SaveSettingsData();
        LoadScene(GameScene.MainMenu, true);
    }

    public GameScene GetCurrentScene()
    {
        if (System.Enum.TryParse(SceneManager.GetActiveScene().name, out GameScene result))
            return result;
        return GameScene.Persistent;
    }

    private void SaveSettingsData()
    {
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
            AudioManager.Instance.SetBGMVolume(data.bgmVolume);
            AudioManager.Instance.SetUIVolume(data.uiVolume);
            QualitySettings.SetQualityLevel(data.qualityPreset, true);
        }
    }

    // 🎵 BGM Switcher
    private void PlaySceneBGM(GameScene scene)
    {
        if (AudioManager.Instance == null) return;

        switch (scene)
        {
            case GameScene.MainMenu:
                AudioManager.Instance.PlayBGM("MainMenuTheme");
                break;
            case GameScene.Level1:
                AudioManager.Instance.PlayBGM("Level1Theme");
                break;
            case GameScene.Level2:
                AudioManager.Instance.PlayBGM("Level2Theme");
                break;
            default:
                AudioManager.Instance.StopBGM();
                break;
        }
    }
}
