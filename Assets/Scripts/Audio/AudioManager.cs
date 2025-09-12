using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private SoundData soundData;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private int sfxPoolSize = 10;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = true;

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private float masterVolume = 1f, bgmVolume = 1f, sfxVolume = 1f, uiVolume = 1f;
    private Coroutine crossfadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (debugMode)
            Debug.Log("[AudioManager] Awake called");

        if (bgmSource == null)
        {
            var bgmObj = new GameObject("BGM_Source");
            bgmObj.transform.SetParent(transform);
            bgmSource = bgmObj.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            if (debugMode)
                Debug.Log("[AudioManager] Created missing BGM AudioSource.");
        }

        if (uiSource == null)
        {
            var uiObj = new GameObject("UI_Source");
            uiObj.transform.SetParent(transform);
            uiSource = uiObj.AddComponent<AudioSource>();
            uiSource.playOnAwake = false;
            if (debugMode)
                Debug.Log("[AudioManager] Created missing UI AudioSource.");
        }

        InitSFXPool();
        LoadSettings();
    }

    private void InitSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var obj = new GameObject("SFX_" + i);
            obj.transform.SetParent(transform);
            var src = obj.AddComponent<AudioSource>();
            sfxPool.Add(src);
            if (debugMode)
                Debug.Log($"[AudioManager] SFX pool source {i} initialized.");
        }
    }

    public void PlayBGM(string key, float fadeDuration = 1f)
    {
        var clip = soundData.GetClip(key);
        if (clip == null)
        {
            if (debugMode) Debug.LogError($"[AudioManager] PlayBGM failed: key '{key}' not found.");
            return;
        }

        if (bgmSource == null || !bgmSource.enabled || !bgmSource.gameObject.activeInHierarchy)
        {
            Debug.LogError("[AudioManager] BGM AudioSource not available.");
            return;
        }

        if (debugMode) Debug.Log($"[AudioManager] Playing BGM '{key}' → clip '{clip.name}'");

        if (crossfadeRoutine != null)
            StopCoroutine(crossfadeRoutine);

        if (bgmSource.isPlaying)
            crossfadeRoutine = StartCoroutine(CrossfadeBGM(clip, key, fadeDuration));
        else
        {
            bgmSource.clip = clip;
            bgmSource.loop = soundData.IsLooping(key);
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.Play();
            if (debugMode)
                Debug.Log($"[AudioManager] BGM '{clip.name}' started at volume {bgmSource.volume}");
        }
    }

    private IEnumerator CrossfadeBGM(AudioClip newClip, string key, float duration)
    {
        float startVol = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVol, 0, t / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = soundData.IsLooping(key);
        bgmSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, bgmVolume * masterVolume, t / duration);
            yield return null;
        }

        if (debugMode)
            Debug.Log($"[AudioManager] Crossfade complete, playing '{newClip.name}'");
    }

    public void PlaySFX(string key, float pitchVar = 0.1f)
    {
        var clip = soundData.GetClip(key);
        if (clip == null)
        {
            if (debugMode) Debug.LogError($"[AudioManager] PlaySFX failed: key '{key}' not found.");
            return;
        }

        var src = GetAvailableSFXSource();
        src.clip = clip;
        src.loop = false;
        src.volume = sfxVolume * masterVolume;
        src.pitch = 1f + Random.Range(-pitchVar, pitchVar);
        src.Play();

        if (debugMode)
            Debug.Log($"[AudioManager] Played SFX '{key}' with volume {src.volume}");
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
                return src;
        }
        if (debugMode)
            Debug.LogWarning("[AudioManager] All SFX sources busy, using first in pool.");
        return sfxPool[0];
    }

    public void PlayUI(string key)
    {
        var clip = soundData.GetClip(key);
        if (clip == null)
        {
            if (debugMode) Debug.LogError($"[AudioManager] PlayUI failed: key '{key}' not found.");
            return;
        }
        if (uiSource == null || !uiSource.enabled || !uiSource.gameObject.activeInHierarchy)
        {
            Debug.LogError("[AudioManager] UI AudioSource not available.");
            return;
        }
        uiSource.PlayOneShot(clip, uiVolume * masterVolume);
        if (debugMode)
            Debug.Log($"[AudioManager] Played UI sound '{key}'");
    }

    public void SetMasterVolume(float v)
    {
        masterVolume = v;
        ApplyVolumes();
        SaveSettings();
    }

    public void SetBGMVolume(float v)
    {
        bgmVolume = v;
        ApplyVolumes();
        SaveSettings();
    }

    public void SetSFXVolume(float v)
    {
        sfxVolume = v;
        ApplyVolumes();
        SaveSettings();
    }

    public void SetUIVolume(float v)
    {
        uiVolume = v;
        ApplyVolumes();
        SaveSettings();
    }

    public float GetVolume(string t) => t switch
    {
        "BGM" => bgmVolume,
        "SFX" => sfxVolume,
        "UI" => uiVolume,
        "Master" => masterVolume,
        _ => 0,
    };

    public void SetVolume(string t, float value)
    {
        switch (t)
        {
            case "BGM": bgmVolume = value; break;
            case "SFX": sfxVolume = value; break;
            case "UI": uiVolume = value; break;
            case "Master": masterVolume = value; break;
        }
        ApplyVolumes();
        SaveSettings();
    }

    private void ApplyVolumes()
    {
        if (bgmSource) bgmSource.volume = bgmVolume * masterVolume;
        if (uiSource) uiSource.volume = uiVolume * masterVolume;
        foreach (var src in sfxPool)
            src.volume = sfxVolume * masterVolume;
        if (debugMode)
            Debug.Log($"[AudioManager] Volumes applied → Master: {masterVolume}, BGM: {bgmVolume}, SFX: {sfxVolume}, UI: {uiVolume}");
    }

    private void SaveSettings()
    {
        var data = new SettingsSaveData(bgmVolume, uiVolume, QualitySettings.GetQualityLevel(), sfxVolume, masterVolume);
        SaveSystem.Save(SaveType.Settings, data);
        if (debugMode)
            Debug.Log("[AudioManager] Settings saved.");
    }

    private void LoadSettings()
    {
        var data = SaveSystem.Load<SettingsSaveData>(SaveType.Settings);
        if (data != null)
        {
            bgmVolume = data.bgmVolume;
            uiVolume = data.uiVolume;
            sfxVolume = data.sfxVolume;
            masterVolume = data.masterVolume;
            ApplyVolumes();
            if (debugMode)
                Debug.Log("[AudioManager] Settings loaded successfully.");
        }
    }

    public void StopBGM() => bgmSource?.Stop();
    public void PauseBGM() => bgmSource?.Pause();
}
