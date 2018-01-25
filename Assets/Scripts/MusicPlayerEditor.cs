using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MusicPlayer))]
public class MusicPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {        
        DrawDefaultInspector();

        // Add space
        EditorGUILayout.Space();

        MusicPlayer musicPlayer = (MusicPlayer)target;

        if (musicPlayer.autoPopulateTracks)
        {
            musicPlayer.PopulateTracksWithChildren();
        }

        if (GUILayout.Button("Populate tracks with children"))
        {
            musicPlayer.PopulateTracksWithChildren();
        }
    }
}
