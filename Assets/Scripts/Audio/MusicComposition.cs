using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicMixer
{
    /// <summary>
    /// A collection of music tracks part of the same music composition.
    /// Employs a base track that is meant to always be active with additional accompanying tracks that can be faded in and out.
    /// </summary>
    [Serializable]
    public class MusicComposition
    {
        [SerializeField]
        public int baseTrackIndex;

        [SerializeField]
        public int[] accompanyingTracks;

        public bool expandInEditor;

        private MusicPlayer musicPlayer;

        public MusicPlayer MusicPlayer
        {
            get
            {
                if (musicPlayer != null)
                {
                    return musicPlayer;
                }
                else if (MusicPlayer.Instance == null)
                {
                    Debug.LogError("No MusicPlayer available.");
                    return null;
                }
                else
                {
                    musicPlayer = MusicPlayer.Instance;
                    return musicPlayer;
                }
            }
        }

        // TODO: Add functionality to stop composition tracks after a set amount of time. Suggestion: Co-routine?

        public MusicComposition(MusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
        }

        /// <summary>
        /// Begins playing base track and accompanying tracks, setting volume 
        /// </summary>
        public void ActivateGroup()
        {
            ActivateTrack(baseTrackIndex);
            foreach (int track in accompanyingTracks)
            {
                // TODO: Find and use appropriate target volume
                ActivateTrack(track, 0f);
            }
        }

        /// <summary>
        /// Begins playing track and fade in at full volume
        /// </summary>
        /// <param name="index">Index in MusicPlayer's track list</param>
        private void ActivateTrack(int index)
        {
            ActivateTrack(index, 1f);
        }

        /// <summary>
        /// Begins playing track and fade
        /// </summary>
        /// <param name="index">Index in MusicPlayer's track list</param>
        /// <param name="volume">Volume to fade to</param>
        public void ActivateTrack(int index, float volume)
        {
            MusicTrack activatingMusicTrack = GetMusicTrack(index);
            musicPlayer.PlayTrack(activatingMusicTrack, volume);
        }

        public void DeactivateGroup()
        {
            DeactivateTrack(baseTrackIndex);
            foreach (int trackIndex in accompanyingTracks)
            {
                DeactivateTrack(trackIndex);
            }
        }

        private void DeactivateTrack(int baseTrackIndex)
        {
            MusicTrack deactivatingMusicTrack = GetMusicTrack(baseTrackIndex);
            deactivatingMusicTrack.StartFade(0f, deactivatingMusicTrack.FadeDuration);
        }

        private MusicTrack GetMusicTrack(int trackIndex)
        {
            bool indexWithinBounds = MusicPlayer.Tracks.Count > trackIndex;
            if (!indexWithinBounds)
            {
                // TODO: Implement exception
                throw new NotImplementedException();
            }
            else
            {
                return MusicPlayer.Tracks[trackIndex];
            }
        }

        public override string ToString()
        {
            string baseTrackName = MusicPlayer.Tracks[baseTrackIndex].ToString();
            baseTrackName += " - Composition";
            return baseTrackName;
        }
    }
}