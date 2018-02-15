using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MusicMixer
{
    [CustomEditor(typeof(MusicComposition))]
    public class MusicCompositionEditor : Editor
    {
        private int selectedIndex;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            Debug.Log("Drawing OnInspectorGUI for MusicComposition.");

            // Get our testing object
            //MusicControllerTester tester = (MusicControllerTester)target;

            // Set selectedIndex based on 
            selectedIndex = EditorGUILayout.Popup(selectedIndex, MusicController.GetAllTracksAsString());
            
        }
    } 
}
