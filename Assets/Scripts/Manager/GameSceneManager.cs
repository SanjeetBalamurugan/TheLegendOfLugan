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
    [SerializeField] private GameScene nextSceneToLoad;

    public static event System.Action<GameScene> OnSceneLoaded;

    private void Start()
    {
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
        LoadScene(GameScene.MainMenu, true);
    }

    public GameScene GetCurrentScene()
    {
        return (GameScene)System.Enum.Parse(typeof(GameScene), SceneManager.GetActiveScene().name);
    }
}
