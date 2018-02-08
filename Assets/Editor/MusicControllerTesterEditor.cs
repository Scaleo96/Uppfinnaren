using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MusicMixer;

[CustomEditor(typeof(MusicControllerTester))]
public class MusicControllerTesterEditor : Editor {

    int selectedInt;
    public override void OnInspectorGUI()
    {
        

        MusicControllerTester tester = (MusicControllerTester)target;

        List<string> trackStrings = new List<string>();
        foreach (MusicTrack track in MusicController.GetAllTracks())
        {
            trackStrings.Add(track.ToString());
        }

        selectedInt = EditorGUILayout.Popup(selectedInt, trackStrings.ToArray());

        DrawDefaultInspector();

        if (GUILayout.Button("TEST - Start fade on track"))
        {
            MusicController.FadeTrack(selectedInt, tester.targetVolume, tester.fadeDuration);
        }
    }
}
