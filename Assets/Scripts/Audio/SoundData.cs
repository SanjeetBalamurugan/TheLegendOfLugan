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
    public bool debugMode = false;

    private void OnEnable()
    {
        if (debugMode)
        {
            foreach (var sound in sounds)
            {
                if (string.IsNullOrEmpty(sound.key))
                    Debug.LogWarning("[SoundData] Empty key found in entry.");

                if (sound.clip == null)
                    Debug.LogWarning($"[SoundData] Null AudioClip for key '{sound.key}'.");
                else
                    Debug.Log($"[SoundData] Registered sound key '{sound.key}' with clip '{sound.clip.name}', loop: {sound.loop}");
            }
        }
    }

    public AudioClip GetClip(string key)
    {
        foreach (var sound in sounds)
        {
            if (sound.key == key)
            {
                if (debugMode)
                    Debug.Log($"[SoundData] GetClip successful: '{key}' → '{sound.clip.name}'");
                return sound.clip;
            }
        }

        Debug.LogWarning($"[SoundData] GetClip failed: Sound with key '{key}' not found.");
        return null;
    }

    public bool IsLooping(string key)
    {
        foreach (var sound in sounds)
        {
            if (sound.key == key)
                return sound.loop;
        }

        if (debugMode)
            Debug.LogWarning($"[SoundData] IsLooping check failed: Key '{key}' not found.");
        return false;
    }
}
