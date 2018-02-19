using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicMixer
{
    [Serializable]
    public class MusicComposition
    {
        [SerializeField]
        public int baseTrackIndex;

        [SerializeField]
        public int[] accompanyingTracks;

        private MusicPlayer musicPlayer;

        public MusicComposition(MusicPlayer musicPlayer)
        {
            this.musicPlayer = musicPlayer;
        }

        /// <summary>
        /// Begins playing base track and accompanying tracks, setting volume 
        /// </summary>
        private void ActivateGroup()
        {
            ActivateTrack(baseTrackIndex);
            foreach (var track in accompanyingTracks)
            {
                ActivateTrack(track);
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
        private void ActivateTrack(int index, float volume)
        {
            MusicTrack activatingMusicTrack = musicPlayer.Tracks[index];
            musicPlayer.PlayTrack(activatingMusicTrack, volume);
        }
    }
}