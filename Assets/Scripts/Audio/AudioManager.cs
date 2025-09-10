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

    private List<AudioSource> sfxPool = new List<AudioSource>();
    private float masterVolume = 1f, bgmVolume = 1f, sfxVolume = 1f, uiVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitSFXPool();
        LoadSettings();
    }

    private void InitSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var obj = new GameObject("SFX_" + i);
            obj.transform.SetParent(transform);
            sfxPool.Add(obj.AddComponent<AudioSource>());
        }
    }

    public void PlayBGM(string key, float fadeDuration = 1f)
    {
        var clip = soundData.GetClip(key);
        if (clip == null) return;

        if (bgmSource.isPlaying)
            StartCoroutine(CrossfadeBGM(clip, key, fadeDuration));
        else
        {
            bgmSource.clip = clip;
            bgmSource.loop = soundData.IsLooping(key);
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.Play();
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
    }

    public void PlaySFX(string key, float pitchVar = 0.1f)
    {
        var clip = soundData.GetClip(key);
        if (clip == null) return;

        var src = GetAvailableSFXSource();
        src.clip = clip;
        src.loop = false;
        src.volume = sfxVolume * masterVolume;
        src.pitch = 1f + Random.Range(-pitchVar, pitchVar);
        src.Play();
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var src in sfxPool)
            if (!src.isPlaying) return src;
        return sfxPool[0];
    }

    public void PlayUI(string key)
    {
        var clip = soundData.GetClip(key);
        if (clip == null) return;
        uiSource.PlayOneShot(clip, uiVolume * masterVolume);
    }

    public void SetMasterVolume(float v) { masterVolume = v; ApplyVolumes(); SaveSettings(); }
    public void SetBGMVolume(float v) { bgmVolume = v; ApplyVolumes(); SaveSettings(); }
    public void SetSFXVolume(float v) { sfxVolume = v; ApplyVolumes(); SaveSettings(); }
    public void SetUIVolume(float v) { uiVolume = v; ApplyVolumes(); SaveSettings(); }

    private void ApplyVolumes()
    {
        if (bgmSource) bgmSource.volume = bgmVolume * masterVolume;
        if (uiSource) uiSource.volume = uiVolume * masterVolume;
        foreach (var src in sfxPool)
            src.volume = sfxVolume * masterVolume;
    }

    private void SaveSettings()
    {
        var data = new SettingsSaveData(bgmVolume, uiVolume, QualitySettings.GetQualityLevel(), sfxVolume, masterVolume);
        SaveSystem.Save(SaveType.Settings, data);
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
        }
    }
}
