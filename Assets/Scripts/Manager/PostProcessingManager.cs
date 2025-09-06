using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume postProcessVolume;

    [Header("Toggles")]
    [SerializeField] private bool cinematicMode = false;

    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private LiftGammaGain liftGammaGain;
    private AmbientOcclusion ambientOcclusion;
    private Vignette vignette;
    private DepthOfField depthOfField;

    private void Start()
    {
        if (postProcessVolume == null || postProcessVolume.profile == null)
        {
            Debug.LogError("PostProcessingManager: Volume reference missing!");
            return;
        }

        postProcessVolume.profile.TryGet(out colorAdjustments);
        postProcessVolume.profile.TryGet(out bloom);
        postProcessVolume.profile.TryGet(out liftGammaGain);
        postProcessVolume.profile.TryGet(out ambientOcclusion);
        postProcessVolume.profile.TryGet(out vignette);
        postProcessVolume.profile.TryGet(out depthOfField);

        ApplyDefaultSettings();
        ApplyCinematicSettings(cinematicMode);
    }

    private void ApplyDefaultSettings()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = 0.5f;
            colorAdjustments.contrast.value = 20f;
            colorAdjustments.saturation.value = 12f;
            colorAdjustments.colorFilter.value = new Color(1f, 0.96f, 0.88f); // warm filter
        }

        if (bloom != null)
        {
            bloom.intensity.value = 1.4f;
            bloom.threshold.value = 1.2f;
            bloom.scatter.value = 0.65f;
        }

        if (liftGammaGain != null)
        {
            liftGammaGain.lift.value = new Vector4(0.97f, 0.97f, 0.97f, 0);
            liftGammaGain.gamma.value = new Vector4(1.05f, 1.05f, 1.05f, 0);
            liftGammaGain.gain.value = new Vector4(1.12f, 1.12f, 1.12f, 0);
        }

        if (ambientOcclusion != null)
        {
            ambientOcclusion.intensity.value = 0.5f;
            ambientOcclusion.quality.value = 2; // medium
        }

        if (vignette != null)
        {
            vignette.intensity.value = 0.15f;
            vignette.smoothness.value = 0.35f;
        }

        if (depthOfField != null)
        {
            depthOfField.active = false; // disabled by default
        }
    }

    public void ApplyCinematicSettings(bool enable)
    {
        if (vignette != null)
        {
            vignette.intensity.value = enable ? 0.3f : 0.15f;
            vignette.smoothness.value = enable ? 0.5f : 0.35f;
        }

        if (depthOfField != null)
        {
            depthOfField.active = enable;
            if (enable)
            {
                depthOfField.mode.value = DepthOfFieldMode.Bokeh;
                depthOfField.focusDistance.value = 6f;
                depthOfField.focalLength.value = 50f;
                depthOfField.aperture.value = 4f;
            }
        }
    }

    private void Update()
    {
        ApplyCinematicSettings(cinematicMode);
    }
}
