using UnityEngine;
using UnityEditor;

public class AudioManagerWindow : EditorWindow
{
    private SoundData soundData;
    private Vector2 scrollPos;
    private AudioSource previewSource;

    [MenuItem("Window/Audio Manager")]
    public static void ShowWindow()
    {
        GetWindow<AudioManagerWindow>("Audio Manager");
    }

    private void OnEnable()
    {
        if (!previewSource)
        {
            GameObject go = new GameObject("AudioPreviewSource");
            go.hideFlags = HideFlags.HideAndDontSave;
            previewSource = go.AddComponent<AudioSource>();
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Audio Manager Tool", EditorStyles.boldLabel);

        soundData = (SoundData)EditorGUILayout.ObjectField("Sound Data", soundData, typeof(SoundData), false);

        if (soundData == null)
        {
            EditorGUILayout.HelpBox("Assign a SoundData asset to view and preview sounds.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var sound in soundData.sounds)
        {
            EditorGUILayout.BeginHorizontal("box");

            GUILayout.Label(sound.key, GUILayout.Width(150));

            if (GUILayout.Button("▶️ Play", GUILayout.Width(60)))
            {
                PlayClip(sound.clip, sound.volume, sound.pitch);
            }

            if (GUILayout.Button("⏹ Stop", GUILayout.Width(60)))
            {
                StopClip();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void PlayClip(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;
        previewSource.pitch = pitch;
        previewSource.volume = volume;
        previewSource.clip = clip;
        previewSource.Play();
    }

    private void StopClip()
    {
        if (previewSource.isPlaying)
        {
            previewSource.Stop();
        }
    }
}
