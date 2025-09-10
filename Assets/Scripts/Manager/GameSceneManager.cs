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
        LoadSettingsData();
    }

    private IEnumerator LoadGameSequence()
    {
        if (!SceneManager.GetSceneByName(GameScene.Persistent.ToString()).isLoaded)
        {
            if (Application.CanStreamedLevelBeLoaded(GameScene.Persistent.ToString()))
                yield return SceneManager.LoadSceneAsync(GameScene.Persistent.ToString(), LoadSceneMode.Additive);
            else
                yield break;
        }

        if (useLoadingScreen)
        {
            if (Application.CanStreamedLevelBeLoaded(GameScene.LoadingScreen.ToString()))
                yield return SceneManager.LoadSceneAsync(GameScene.LoadingScreen.ToString(), LoadSceneMode.Additive);
            else
                yield break;
        }

        yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));

        if (useLoadingScreen && SceneManager.GetSceneByName(GameScene.LoadingScreen.ToString()).isLoaded)
            SceneManager.UnloadSceneAsync(GameScene.LoadingScreen.ToString());
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

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        currentScene = scene;
        yield return null;
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
        GameScene scene = (GameScene)System.Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name);
        LoadScene(scene, useLoadingScreen);
    }

    public void QuitToMainMenu()
    {
        SaveSettingsData();
        LoadScene(GameScene.MainMenu, true);
    }

    public GameScene GetCurrentScene()
    {
        return (GameScene)System.Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name);
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
}
