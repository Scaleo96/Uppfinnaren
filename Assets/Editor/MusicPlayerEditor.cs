using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MusicMixer
{
    [CustomEditor(typeof(MusicPlayer))]
    public class MusicPlayerEditor : Editor
    {
        [SerializeField]
        private bool showAdvancedSettings;

        private MusicPlayer musicPlayer;
        private List<string> trackNames = new List<string>();

        private GUIStyle boldFoldout;

        public override void OnInspectorGUI()
        {
            musicPlayer = (MusicPlayer)target;

            SetupLayoutStyles();

            ToggleAdvancedSettings();

            if (!showAdvancedSettings)
            {
                DrawCustomInspector();
            }
            else
            {
                DrawDefaultInspector();
            }

            // Add space
            EditorGUILayout.Space();

            if (musicPlayer.compositions != null)
            {
                MusicCompositions();
            }

            if (showAdvancedSettings)
            {
                DrawButtons();
            }

            musicPlayer.UpdateInfo();
        }

        private void SetupLayoutStyles()
        {
            boldFoldout = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold
            };
        }

        private void ToggleAdvancedSettings()
        {
            musicPlayer.showAdvancedSettings = EditorGUILayout.Toggle("Show advanced options", musicPlayer.showAdvancedSettings);

            showAdvancedSettings = musicPlayer.showAdvancedSettings;
        }

        private void DrawCustomInspector()
        {
            musicPlayer.expandMusicTracks = EditorGUILayout.Foldout(musicPlayer.expandMusicTracks, "Music Tracks", true, boldFoldout);

            if (musicPlayer.expandMusicTracks)
            {
                DrawCustomMusicTracks();
            }
        }

        private void DrawCustomMusicTracks()
        {
            EditorGUILayout.BeginVertical();
            foreach (MusicTrack track in musicPlayer.Tracks)
            {
                if (track == null)
                {
                    break;
                }

                // Foldout
                track.expandInEditor = EditorGUILayout.Foldout(track.expandInEditor, track.ToString(), true, boldFoldout);

                if (track.expandInEditor)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Target Volume");
                    track.TargetVolume = EditorGUILayout.Slider(track.TargetVolume, 0f, 1f);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Current volume");

                    Rect r = EditorGUILayout.BeginVertical();
                    EditorGUI.ProgressBar(r, track.Volume, "");
                    GUILayout.Space(18);
                    EditorGUILayout.EndVertical();
                    Handles.EndGUI();

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Fade duration");
                    track.FadeDuration = EditorGUILayout.DelayedFloatField(track.FadeDuration);
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }

        private void MusicCompositions()
        {
            RefreshTrackNames();

            EditorGUILayout.LabelField("Music compositions", EditorStyles.boldLabel);

            // Size of music compos
            SetSizeOfCompositionsArray();

            foreach (MusicComposition composition in musicPlayer.compositions)
            {
                if (composition == null)
                {
                    break;
                }

                string baseTrackName = musicPlayer.Tracks[composition.baseTrackIndex].ToString();

                // Foldout
                composition.expandInEditor = EditorGUILayout.Foldout(composition.expandInEditor, "Music composition - " + baseTrackName, true, boldFoldout);
                if (composition.expandInEditor)
                {
                    DrawTrackSelection(composition);
                }

                EditorGUILayout.Separator();
            }
        }

        private void DrawTrackSelection(MusicComposition composition)
        {
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
                Array.Resize<MusicComposition>(ref musicPlayer.compositions, arraySize);
            }
        }

        private static void ResizeOfAccompanyingTracks(MusicComposition composition)
        {
            if (composition.accompanyingTracks == null)
            {
                composition.accompanyingTracks = new int[0];
            }

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