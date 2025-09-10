using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorManager : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button level2Button;

    [Header("Game Manager Reference")]
    [SerializeField] private GameSceneManager gameManager;

    private void Start()
    {
        if (level1Button != null)
            level1Button.onClick.AddListener(() => LoadLevel(GameScene.Level1));

        if (level2Button != null)
            level2Button.onClick.AddListener(() => LoadLevel(GameScene.Level2));
    }

    private void LoadLevel(GameScene scene)
    {
        if (gameManager == null)
        {
            Debug.LogError("GameSceneManager reference is missing!");
            return;
        }

        gameManager.LoadScene(scene, true);
    }
}
