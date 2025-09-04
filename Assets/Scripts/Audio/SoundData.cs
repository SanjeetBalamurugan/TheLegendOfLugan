using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/Sound Data")]
public class SoundData : ScriptableObject
{
    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
        public bool loop;

        public float volume = 1f;
        public float pitch = 1f;
    }

    public SoundEntry[] sounds;

    public AudioClip GetClip(string key)
    {
        foreach (var sound in sounds)
        {
            if (sound.key == key)
                return sound.clip;
        }
        Debug.LogWarning($"Sound with key '{key}' not found.");
        return null;
    }

    public bool IsLooping(string key)
    {
        foreach (var sound in sounds)
        {
            if (sound.key == key)
                return sound.loop;
        }
        return false;
    }
}
