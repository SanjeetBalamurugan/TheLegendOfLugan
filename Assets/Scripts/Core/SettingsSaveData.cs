using System;

[Serializable]
public class SettingsSaveData
{
    public float bgmVolume;
    public float uiVolume;
    public int qualityPreset;

    public SettingsSaveData(float bgm, float ui, int preset)
    {
        bgmVolume = bgm;
        uiVolume = ui;
        qualityPreset = preset;
    }
}
