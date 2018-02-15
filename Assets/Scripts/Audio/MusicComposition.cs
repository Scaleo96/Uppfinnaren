using System;
using System.Collections.Generic;
using UnityEngine;

namespace MusicMixer
{
    [Serializable]
    public class MusicComposition
    {
        [SerializeField]
        private int baseTrackIndex;

        [SerializeField]
        private int accompanyingTracksNumbers;

        private Dictionary<int, float> accompanyingTracks;

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
                ActivateTrack(track.Key, track.Value);
            }
        }

        private void ActivateTrack(int index)
        {
            ActivateTrack(index, 1f);
        }

        private void ActivateTrack(int index, float volume)
        {
            MusicTrack activatingMusicTrack = musicPlayer.Tracks[index];
            musicPlayer.PlayTrack(activatingMusicTrack, volume);
        }
    }
}