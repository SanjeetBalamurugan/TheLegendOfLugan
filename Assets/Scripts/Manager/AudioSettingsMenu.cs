using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider uiSlider;

    private void Start()
    {
        // Initialize slider values from AudioManager
        if (AudioManager.Instance != null)
        {
            bgmSlider.value = AudioManager.Instance.GetVolume("BGM");
            uiSlider.value = AudioManager.Instance.GetVolume("UI");
        }

        // Add listeners
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        uiSlider.onValueChanged.AddListener(SetUIVolume);
    }

    private void SetBGMVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume("BGM", value);
    }

    private void SetUIVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetVolume("UI", value);
    }

    private void OnDestroy()
    {
        // Clean up listeners
        bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
        uiSlider.onValueChanged.RemoveListener(SetUIVolume);
    }
}
