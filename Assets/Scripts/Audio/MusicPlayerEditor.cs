using UnityEditor;
using UnityEngine;

namespace MusicMixer
{
    [CustomEditor(typeof(MusicPlayer))]
    public class MusicPlayerEditor : Editor
    {
        [SerializeField]
        bool testBool;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Add space
            EditorGUILayout.Space();

            MusicPlayer musicPlayer = (MusicPlayer)target;

            if (musicPlayer.AutoPopulateTracks)
            {
                musicPlayer.PopulateTracksWithChildren();
            }

            if (musicPlayer.AutomaticRenaming)
            {
                musicPlayer.RenameObjectsToSourceFile();
            }

            if (GUILayout.Button("Set default fade duration on all tracks"))
            {
                musicPlayer.SetDefaultFadeDurationOnAllTracks();
            }

            if (GUILayout.Button("Populate tracks with children"))
            {
                musicPlayer.PopulateTracksWithChildren();
            }

            if (GUILayout.Button("Rename children to source file"))
            {
                musicPlayer.RenameObjectsToSourceFile();
            }
            
            
            if (GUILayout.Button("Initialize fade on all tracks"))
            {
                musicPlayer.FadeAll();
            }

            musicPlayer.UpdateInfo();
        }
    }

    /// <summary>
    /// Read only attribute
    /// TODO: Make available project wide
    /// </summary>

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

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
}