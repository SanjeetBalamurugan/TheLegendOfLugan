using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Library")]
    [SerializeField] private SoundData soundData;

    [Header("Channels")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private int sfxPoolSize = 10;

    private List<AudioSource> sfxPool = new List<AudioSource>();

    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float sfxVolume = 1f;
    private float uiVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitSFXPool();
        LoadVolumes();
    }

    private void InitSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject obj = new GameObject("SFX_AudioSource_" + i);
            obj.transform.SetParent(transform);
            AudioSource src = obj.AddComponent<AudioSource>();
            sfxPool.Add(src);
        }
    }

    public void PlayBGM(string key, float fadeDuration = 1f)
    {
        AudioClip clip = soundData.GetClip(key);
        if (clip == null) return;

        if (bgmSource.isPlaying)
            StartCoroutine(CrossfadeBGM(clip, fadeDuration));
        else
        {
            bgmSource.clip = clip;
            bgmSource.loop = soundData.IsLooping(key);
            bgmSource.volume = bgmVolume * masterVolume;
            bgmSource.Play();
        }
    }

    private IEnumerator CrossfadeBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, bgmVolume * masterVolume, t / duration);
            yield return null;
        }
    }

    public void PlaySFX(string key, float pitchVariation = 0.1f)
    {
        AudioClip clip = soundData.GetClip(key);
        if (clip == null) return;

        AudioSource src = GetAvailableSFXSource();
        src.clip = clip;
        src.loop = false;
        src.volume = sfxVolume * masterVolume;
        src.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        src.Play();
    }

    private AudioSource GetAvailableSFXSource()
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
                return src;
        }
        return sfxPool[0];
    }

    public void PlayUI(string key)
    {
        AudioClip clip = soundData.GetClip(key);
        if (clip == null) return;

        uiSource.PlayOneShot(clip, uiVolume * masterVolume);
    }

    public void SetMasterVolume(float value) { masterVolume = value; ApplyVolumes(); SaveVolumes(); }
    public void SetBGMVolume(float value) { bgmVolume = value; ApplyVolumes(); SaveVolumes(); }
    public void SetSFXVolume(float value) { sfxVolume = value; ApplyVolumes(); SaveVolumes(); }
    public void SetUIVolume(float value) { uiVolume = value; ApplyVolumes(); SaveVolumes(); }

    private void ApplyVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume * masterVolume;
        if (uiSource != null) uiSource.volume = uiVolume * masterVolume;
        foreach (var src in sfxPool)
            src.volume = sfxVolume * masterVolume;
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat("MasterVol", masterVolume);
        PlayerPrefs.SetFloat("BGMVol", bgmVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
        PlayerPrefs.SetFloat("UIVol", uiVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVol", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGMVol", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVol", 1f);
        uiVolume = PlayerPrefs.GetFloat("UIVol", 1f);
        ApplyVolumes();
    }
}
