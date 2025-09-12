using System;

[Serializable]
public class SettingsSaveData
{
    public float bgmVolume;
    public float uiVolume;
    public float sfxVolume;
    public float masterVolume;
    public int qualityPreset;

    public SettingsSaveData(float bgm, float ui, int preset, float sfx = 1f, float master = 1f)
    {
        bgmVolume = bgm;
        uiVolume = ui;
        sfxVolume = sfx;
        masterVolume = master;
        qualityPreset = preset;
    }
}
