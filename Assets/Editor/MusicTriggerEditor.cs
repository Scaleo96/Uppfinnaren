using UnityEditor;

namespace MusicMixer
{
    [CustomEditor(typeof(MusicTrigger))]
    public class MusicTriggerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            // Get our testing object
            MusicTrigger musicTrigger = (MusicTrigger)target;

            DrawDefaultInspector();

            // Set selectedIndex based on 
            musicTrigger.selectedComposition = EditorGUILayout.Popup(musicTrigger.selectedComposition, MusicController.GetAllCompositionsAsString());

            musicTrigger.InitializeComponents();

        }
    }
}