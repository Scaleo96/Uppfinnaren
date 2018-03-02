using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MusicMixer
{
    /// <summary>
    /// Properties of music tracks
    /// </summary>
    [Serializable]
    public class MusicTrack
    {
        [SerializeField]
        public AudioSource trackSource;

        [ReadOnly]
        [SerializeField]
        private AudioClip trackClip;

        [ReadOnly]
        [SerializeField]
        private float currentVolume;

        [SerializeField]
        [Range(0, 1)]
        private float targetVolume = 0;
        [SerializeField]
        private float fadeDuration = 0;

        /// <summary>Monitors how long the track has been fading to new TargetVolume</summary>
        private float fadeTimer;

        /// <summary>Volume to fade from</summary>
        private float fadeStartVolume;

        public MusicTrack(AudioSource track, AudioMixerGroup mixerGroup, bool loop = true)
        {
            trackSource = track;

            // Set the AudioMixerGroup to use the music group
            track.outputAudioMixerGroup = mixerGroup;

            // Set looping
            track.loop = loop;

            UpdateEditorInfo();
        }

        public MusicTrack(AudioSource track, bool loop = true)
        {
            trackSource = track;

            // Set looping
            track.loop = loop;

            UpdateEditorInfo();
        }

        public void StartFade(float newTargetVolume, float newFadeDuration)
        {
            TargetVolume = newTargetVolume;
            FadeDuration = newFadeDuration;
            StartFadeTimer();
        }

        /// <summary>
        /// Change track volume over time, duration set by FadeDuration
        /// </summary>
        public void FadeTrackOverTime()
        {
            if (fadeTimer < FadeDuration)
            {
                // Advance timer
                fadeTimer += Time.unscaledDeltaTime;

                float fadeProgress = Mathf.Clamp01(fadeTimer / FadeDuration);

                // Set volume
                Volume = Mathf.Lerp(fadeStartVolume, TargetVolume, fadeProgress);

                if (Volume == TargetVolume)
                {
                    StopFadeTimer();
                    // "End" timer
                    fadeTimer = float.MaxValue;
                }
            }
            else if (fadeTimer != float.MaxValue)
            {
                // Timer over: setting volume
                Volume = TargetVolume;
                StopFadeTimer();
            }
        }

        /// <summary>
        /// Change track volume over time
        /// </summary>
        /// <param name="newFadeDuration">Fade duration in seconds</param>
        public void FadeTrackOverTime(float newFadeDuration)
        {
            if (FadeDuration != newFadeDuration)
            {
                FadeDuration = newFadeDuration;
            }
            FadeTrackOverTime();
        }

        /// <summary>
        /// Restarts fade timer with the current fade duration with the current volume as starting point
        /// </summary>
        public void StartFadeTimer()
        {
            // Reset fadeTime when a new target is set
            fadeTimer = 0f;
            //Debug.Log("Reset fadetimer");
            fadeStartVolume = Volume;
        }

        public void StopFadeTimer()
        {
            fadeTimer = float.MaxValue;
        }

        /// <summary>
        /// Update track information. Mostly used for ReadOnly info and only runs in editor
        /// </summary>
        public void UpdateEditorInfo()
        {
#if UNITY_EDITOR
            // Clip info
            if (trackClip != trackSource.clip)
            {
                trackClip = trackSource.clip;
            }

            if (currentVolume != Volume)
            {
                currentVolume = Volume;
            }
#endif
        }

        public override string ToString()
        {
            string trackName = trackClip.ToString();
            trackName = trackName.Replace(" (UnityEngine.AudioClip)", "");
            return trackName;
        }

        public float Volume
        {
            get
            {
                return trackSource.volume;
            }

            protected set
            {
                trackSource.volume = Mathf.Clamp01(value);
            }
        }

        public float TargetVolume
        {
            get
            {
                targetVolume = Mathf.Clamp01(targetVolume);
                return targetVolume;
            }

            set
            {
                targetVolume = Mathf.Clamp01(value);
            }
        }

        public float FadeDuration
        {
            get
            {
                return fadeDuration;
            }

            set
            {
                fadeDuration = value;
            }
        }
    }
}