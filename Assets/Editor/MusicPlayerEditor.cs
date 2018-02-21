using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MusicMixer
{
    [CustomEditor(typeof(MusicPlayer))]
    public class MusicPlayerEditor : Editor
    {
        // TODO: Implement advanced setting toggle
        [SerializeField]
        private bool showAdvancedSettings;

        private MusicPlayer musicPlayer;
        private List<string> trackNames = new List<string>();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // Add space
            EditorGUILayout.Space();

            musicPlayer = (MusicPlayer)target;

            MusicCompositions();

            DrawButtons();

            musicPlayer.UpdateInfo();
        }

        private void MusicCompositions()
        {
            RefreshTrackNames();

            EditorGUILayout.LabelField("Music compositions", EditorStyles.boldLabel);
            // Size of music compos
            SetSizeOfCompositionsArray();

            foreach (MusicComposition composition in musicPlayer.compositions)
            {
                EditorGUILayout.Separator();
                string baseTrackName = musicPlayer.Tracks[composition.baseTrackIndex].ToString();

                // Foldout
                composition.expandInEditor = EditorGUILayout.Foldout(composition.expandInEditor, "Music composition - " + baseTrackName);
                if (composition.expandInEditor)
                {
                    EditorGUILayout.LabelField("Music composition - " + baseTrackName, EditorStyles.boldLabel);

                    // Select base track
                    EditorGUILayout.PrefixLabel("Base Track");
                    composition.baseTrackIndex = EditorGUILayout.Popup(composition.baseTrackIndex, trackNames.ToArray());

                    ResizeOfAccompanyingTracks(composition);

                    // Select accompanying tracks
                    for (int i = 0; i < composition.accompanyingTracks.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        int trackIndex = composition.accompanyingTracks[i];

                        EditorGUILayout.PrefixLabel("Acomp. track #" + i + ": ");

                        trackIndex = EditorGUILayout.Popup(trackIndex, trackNames.ToArray());

                        composition.accompanyingTracks[i] = trackIndex;
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.Space();
            }
        }

        private void SetSizeOfCompositionsArray()
        {
            MusicComposition[] compArray = musicPlayer.compositions;
            int arraySize = compArray.Length;

            // Editor GUI
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Amount of compositions");
            arraySize = EditorGUILayout.DelayedIntField(arraySize);
            EditorGUILayout.EndHorizontal();

            if (compArray.Length != arraySize)
            {
                // TODO: Cleanup comments
                //MusicComposition[] newArray = new MusicComposition[arraySize - 1];
                //Debug.Log("Attempting resizing");
                Array.Resize<MusicComposition>(ref musicPlayer.compositions, arraySize);

                //for (int i = 0; i < newArray.Length; i++)
                //{
                //    if (compArray.Length > i)
                //    {
                //        newArray[i] = compArray[i];
                //    }
                //}

                //compArray = newArray;
            }
        }

        private static void ResizeOfAccompanyingTracks(MusicComposition composition)
        {
            int arraySize = composition.accompanyingTracks.Length;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Size");
            arraySize = EditorGUILayout.DelayedIntField(arraySize);
            EditorGUILayout.EndHorizontal();
            Array.Resize<int>(ref composition.accompanyingTracks, arraySize);
        }

        private void DrawButtons()
        {
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
        }

        private void RefreshTrackNames()
        {
            if (trackNames != CurrentTrackNamesList)
            {
                trackNames = CurrentTrackNamesList;
            }
        }

        private List<string> CurrentTrackNamesList
        {
            get
            {
                List<string> updatedTrackNames = new List<string>();

                foreach (MusicTrack track in musicPlayer.Tracks)
                {
                    updatedTrackNames.Add(track.ToString());
                }

                return updatedTrackNames;
            }
        }
    }
}