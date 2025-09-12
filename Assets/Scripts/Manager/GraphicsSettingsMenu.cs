using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsMenu : MonoBehaviour
{
    [Header("Graphics Presets")]
    [SerializeField] private Dropdown presetDropdown;

    [Header("Individual Settings")]
    [SerializeField] private Toggle vSyncToggle;
    [SerializeField] private Dropdown shadowQualityDropdown; // 0 = Off, 1 = Low, 2 = Medium, 3 = High
    [SerializeField] private Dropdown antiAliasingDropdown; // 0 = Off, 1 = 2x, 2 = 4x, 3 = 8x
    [SerializeField] private Toggle postProcessingToggle;

    private void Start()
    {
        // Setup Presets
        presetDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>(QualitySettings.names);
        presetDropdown.AddOptions(options);
        presetDropdown.value = QualitySettings.GetQualityLevel();
        presetDropdown.RefreshShownValue();
        presetDropdown.onValueChanged.AddListener(SetQualityPreset);

        // Setup VSync
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        vSyncToggle.onValueChanged.AddListener(SetVSync);

        // Setup Shadows
        shadowQualityDropdown.value = (int)QualitySettings.shadows;
        shadowQualityDropdown.onValueChanged.AddListener(SetShadows);

        // Setup AntiAliasing
        int aaIndex = 0;
        switch (QualitySettings.antiAliasing)
        {
            case 2: aaIndex = 1; break;
            case 4: aaIndex = 2; break;
            case 8: aaIndex = 3; break;
        }
        antiAliasingDropdown.value = aaIndex;
        antiAliasingDropdown.onValueChanged.AddListener(SetAntiAliasing);

        // Setup PostProcessing (using Unity's default pipeline flag)
        postProcessingToggle.isOn = QualitySettings.activeColorSpace == ColorSpace.Linear;
        postProcessingToggle.onValueChanged.AddListener(SetPostProcessing);
    }

    // -------------------------
    // Preset Handling
    // -------------------------
    private void SetQualityPreset(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log($"Graphics preset set to: {QualitySettings.names[index]}");

        // Refresh individual controls to reflect preset values
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        shadowQualityDropdown.value = (int)QualitySettings.shadows;

        int aaIndex = 0;
        switch (QualitySettings.antiAliasing)
        {
            case 2: aaIndex = 1; break;
            case 4: aaIndex = 2; break;
            case 8: aaIndex = 3; break;
        }
        antiAliasingDropdown.value = aaIndex;
    }

    // -------------------------
    // Individual Settings
    // -------------------------
    private void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        Debug.Log("VSync: " + (enabled ? "On" : "Off"));
    }

    private void SetShadows(int index)
    {
        QualitySettings.shadows = (ShadowQuality)index;
        Debug.Log("Shadows set to: " + (ShadowQuality)index);
    }

    private void SetAntiAliasing(int index)
    {
        int value = 0;
        switch (index)
        {
            case 1: value = 2; break;
            case 2: value = 4; break;
            case 3: value = 8; break;
        }
        QualitySettings.antiAliasing = value;
        Debug.Log("AntiAliasing set to: " + (value == 0 ? "Off" : $"{value}x"));
    }

    private void SetPostProcessing(bool enabled)
    {
        // You can link this to your post-processing volume instead of color space
        Debug.Log("Post Processing " + (enabled ? "Enabled" : "Disabled"));
        // Example: PostProcessVolume.enabled = enabled;
    }

    private void OnDestroy()
    {
        presetDropdown.onValueChanged.RemoveListener(SetQualityPreset);
        vSyncToggle.onValueChanged.RemoveListener(SetVSync);
        shadowQualityDropdown.onValueChanged.RemoveListener(SetShadows);
        antiAliasingDropdown.onValueChanged.RemoveListener(SetAntiAliasing);
        postProcessingToggle.onValueChanged.RemoveListener(SetPostProcessing);
    }
}
