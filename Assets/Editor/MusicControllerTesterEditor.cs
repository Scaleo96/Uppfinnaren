using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MusicMixer;

[CustomEditor(typeof(MusicControllerTester))]
public class MusicControllerTesterEditor : Editor {

    int selectedIndex;
    public override void OnInspectorGUI()
    {
        // Get our testing object
        MusicControllerTester tester = (MusicControllerTester)target;

        // Set selectedIndex based on 
        selectedIndex = EditorGUILayout.Popup(selectedIndex, MusicController.GetAllTracksAsString());

        DrawDefaultInspector();

        // Test button
        if (GUILayout.Button("TEST - Start fade on track"))
        {
            // Fade track based on index of selected track, save the name of the track for debugging
            string debugMessage = MusicController.FadeTrack(selectedIndex, tester.targetVolume, tester.fadeDuration);

            Debug.Log("Fading <b>" + debugMessage + "</b> to target volume '" + tester.targetVolume + "' over '" + tester.fadeDuration + "' seconds");
        }
    }
}
