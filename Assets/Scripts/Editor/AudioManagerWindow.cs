using UnityEngine;
using UnityEditor;

public class AudioManagerWindow : EditorWindow
{
    private SoundData soundData;
    private Vector2 scrollPos;
    private AudioClip previewClip;

    [MenuItem("Window/Audio Manager")]
    public static void ShowWindow()
    {
        GetWindow<AudioManagerWindow>("Audio Manager");
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
                PlayClip(sound.clip);
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

    private void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        StopClip();
        previewClip = clip;
        EditorUtility.PlayPreviewClip(previewClip);
    }

    private void StopClip()
    {
        if (previewClip != null)
            EditorUtility.StopPreviewClip();
    }
}
