using System;
using System.Collections;
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

        private int[] activeTracks;

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
                    if (!GameObject.FindObjectOfType<MusicPlayer>())
                    {
                        Debug.LogError("No MusicPlayer available.");
                        return null;
                    }
                    else
                    {
                        return musicPlayer = GameObject.FindObjectOfType<MusicPlayer>();
                    }                    
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
        public void ActivateGroup(int accompanyingTrackIndex)
        {
            PlayTrack(baseTrackIndex);
            for (int i = 0; i < accompanyingTracks.Length; i++)
            {
                // TODO: Find and use appropriate target volume
                if (i != accompanyingTrackIndex)
                {
                    // Activate at 0 volume
                    PlayTrack(accompanyingTracks[i], 0f);
                }
                else
                {
                    // Activate att full volume
                    PlayTrack(accompanyingTracks[i]);
                }
            }
        }

        /// <summary>
        /// Begins playing track and fade in at full volume
        /// </summary>
        /// <param name="index">Index in MusicPlayer's track list</param>
        private void PlayTrack(int index)
        {
            PlayTrack(index, 1f);
        }

        /// <summary>
        /// Begins playing track and fade
        /// </summary>
        /// <param name="index">Index in MusicPlayer's track list</param>
        /// <param name="volume">Volume to fade to</param>
        private void PlayTrack(int index, float volume)
        {
            MusicTrack activatingMusicTrack = GetMusicTrack(index);
            musicPlayer.PlayTrack(activatingMusicTrack, volume);
        }

        /// <summary>
        /// Fades in one accompanying track to full volume and fades out the rest to 0 volume.
        /// Leaves the base track alone.
        /// </summary>
        /// <param name="accompanyingTrackIndex"></param>
        internal void FadeToTrackExlusive(int accompanyingTrackIndex)
        {
            for (int i = 0; i < accompanyingTracks.Length; i++)
            {
                if (i != accompanyingTrackIndex)
                {
                    // Activate at 0 volume
                    PlayTrack(accompanyingTracks[i], 0f);
                }
                else
                {
                    // Activate at full volume
                    SetActiveTracks(i);
                    if (!musicPlayer.CompWaitForFade)
                    {
                        PlayActiveTracks();
                    }
                    else
                    {
                        // HACK: Uses MusicPlayer to start coroutine as MusicCompositions are unable to themselves. They should terminate once the composition changes and the tracks are no longer played, but it's not a guarantee.
                        MusicPlayer.StartCoroutine(PlayActiveTracksDelayed());
                        PlayActiveTracksDelayed();
                    }
                }
            }
        }

        private void SetActiveTracks(int trackIndex)
        {
            activeTracks = new int[1];
            activeTracks[0] = trackIndex;
        }

        private void ClearActiveTracks()
        {
            activeTracks = null;
        }

        private void SetActiveTracks(int[] trackIndices)
        {
            activeTracks = trackIndices;
        }

        public void DeactivateComposition()
        {
            DeactivateTrack(baseTrackIndex);
            foreach (int trackIndex in accompanyingTracks)
            {
                DeactivateTrack(trackIndex);
            }
            ClearActiveTracks();
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
            if (MusicPlayer == null)
            {
                return null;
            }
            string baseTrackName = MusicPlayer.Tracks[baseTrackIndex].ToString();
            baseTrackName += " - Composition";
            return baseTrackName;
        }

        /// <summary>
        /// Same as PlayActiveTracks except it waits for all other tracks to reach their target volume before starting.
        /// </summary>
        private IEnumerator PlayActiveTracksDelayed()
        {
            while (TracksAreStillFadingOut())
            {
                // TODO: Should this really be running every fixed update?
                yield return new WaitForFixedUpdate();
            }
            PlayActiveTracks();
        }

        private bool TracksAreStillFadingOut()
        {
            bool isFadingOut = false;

            for (int i = 0; i < accompanyingTracks.Length; i++)
            {
                MusicTrack track = GetMusicTrack(accompanyingTracks[i]);
                if (track.TargetVolume + musicPlayer.CompositionFadeTolerance < track.Volume)
                {
                    isFadingOut = true;
                    break;
                }
            }

            return isFadingOut;
        }

        /// <summary>
        /// Plays tracks listed in the activeTracks array at full volume
        /// </summary>
        private void PlayActiveTracks()
        {
            if (activeTracks == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < activeTracks.Length; i++)
                {
                    int trackIndex = activeTracks[i];
                    PlayTrack(accompanyingTracks[trackIndex]);
                }
            }
        }
    }
}