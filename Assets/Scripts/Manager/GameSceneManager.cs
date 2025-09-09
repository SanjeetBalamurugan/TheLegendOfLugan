using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameScene
{
    None,
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
        LoadSettingsData();
    }

    private IEnumerator LoadGameSequence()
    {
        if (!SceneManager.GetSceneByName(GameScene.Persistent.ToString()).isLoaded)
            yield return SceneManager.LoadSceneAsync(GameScene.Persistent.ToString(), LoadSceneMode.Additive);

        if (useLoadingScreen)
        {
            yield return SceneManager.LoadSceneAsync(GameScene.LoadingScreen.ToString(), LoadSceneMode.Additive);

            LoadingScreenManager.Instance.FadeIn();
            yield return new WaitUntil(() => LoadingScreenManager.Instance.IsFullyVisible);

            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));

            yield return new WaitForSeconds(0.5f);
            LoadingScreenManager.Instance.FadeOut();
            yield return new WaitUntil(() => LoadingScreenManager.Instance.IsFullyHidden);

            yield return SceneManager.UnloadSceneAsync(GameScene.LoadingScreen.ToString());
        }
        else
        {
            yield return StartCoroutine(LoadNextSceneAsync(nextSceneToLoad));
        }
    }

    private IEnumerator LoadNextSceneAsync(GameScene scene)
    {
        if (currentScene != GameScene.None && currentScene != GameScene.LoadingScreen)
            yield return SceneManager.UnloadSceneAsync(currentScene.ToString());

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        while (!loadOp.isDone)
        {
            if (LoadingScreenManager.Instance != null)
                LoadingScreenManager.Instance.SetProgress(loadOp.progress);
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.ToString()));
        currentScene = scene;
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
        if (currentScene == GameScene.None) return;
        LoadScene(currentScene, useLoadingScreen);
    }

    public void QuitToMainMenu()
    {
        SaveSettingsData();
        LoadScene(GameScene.MainMenu, true);
    }

    public GameScene GetCurrentScene()
    {
        return currentScene;
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
