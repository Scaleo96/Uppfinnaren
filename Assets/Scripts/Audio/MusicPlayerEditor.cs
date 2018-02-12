#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace MusicMixer
{
#if UNITY_EDITOR
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

            if (GUILayout.Button("Initialize fade on all tracks"))
            {
                musicPlayer.FadeAll();
            }

            musicPlayer.UpdateInfo();
        }
    }
#endif
    /// <summary>
    /// Read only attribute
    /// TODO: Make available project wide
    /// </summary>

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}