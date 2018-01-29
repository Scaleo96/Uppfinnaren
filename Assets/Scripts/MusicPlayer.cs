using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    AudioMixerGroup musicMixer;

    [SerializeField]
    bool startFadingIn;

    [Header("Music Tracks")]

    [SerializeField]
    List<MusicTrack> tracks;
    
    [Header("Batch adding of tracks")]
    [SerializeField]
    public bool autoPopulateTracks;


    private void Update()
    {
        AdjustTrackVolume();
    }

    private void AdjustTrackVolume()
    {
        // Iterate through trackList
        foreach (MusicTrack track in tracks)
        {
            // Check if it's the correct volume
            if (track.Volume != track.TargetVolume)
            {
                // Update fade function on track to continue fading
                track.FadeTrackOverTime();
            }
        }
    }

    private void Awake()
    {
        if (!startFadingIn)
        {
            foreach (MusicTrack track in tracks)
            {
                track.StopFadeTimer();
            }
        }
    }

    /// <summary>
    /// Fades the music track to the desired volume
    /// </summary>
    /// <param name="track">Music track to fade</param>
    /// <param name="targetVolume">Desired volume</param>
    private void BeginTrackFade(MusicTrack track, float targetVolume = .0f)
    {
        track.TargetVolume = targetVolume;
        //track.FadeTrackOverTime();
        track.StartFadeTimer();
    }

    /// <summary>
    /// Fades the music track to the desired volume
    /// </summary>
    /// <param name="track">Music track to fade</param>
    /// <param name="targetVolume">Desired volume</param>
    /// <param name="fadeDuration">New fade duration</param>
    private void BeginTrackFade(MusicTrack track, float targetVolume = .0f, float fadeDuration = .0f)
    {
        track.FadeDuration = fadeDuration;
        BeginTrackFade(track, targetVolume);
    }

    /// <summary>
    /// Fades the music track to the previously set target volume
    /// </summary>
    /// <param name="track">Music track to fade</param>
    private void BeginTrackFade(MusicTrack track)
    {
        BeginTrackFade(track, track.TargetVolume, track.FadeDuration);
    }

    // Find the babies, devour them and put them in our list belly
    // TODO: Fix the comment above
    public void PopulateTracksWithChildren()
    {
        // Make array of all child gameobject with AudioSource components
        AudioSource[] allChildAudioSources = transform.GetComponentsInChildren<AudioSource>();

        RemoveEmptyTracks();
        FindDuplicateTracks();
        
        // TODO: Make check more efficient
        // Check for new sources and add them to the track list
        foreach (AudioSource source in allChildAudioSources)
        {
            // Assume audiosources are new until proven otherwise
            bool isNew = true;

            foreach (MusicTrack track in tracks)
            {
                // Check if a music track with this source already exists
                if (track.trackSource == source)
                {
                    // If duplicate, mark as not new and move on
                    isNew = false;
                    break;
                }
            }

            if (isNew)
            {
                // Create a new MusicTrack with the unique source
                tracks.Add(new MusicTrack(source, musicMixer));
            }
        }

        // TODO: Add sorting
    }

    /// <summary>
    /// Remove tracks with no attached source
    /// </summary>
    private void RemoveEmptyTracks()
    {        
        List<int> safeListOfTracksToRemove = new List<int>();

        // Iterate in reverse
        for (int i = tracks.Count - 1; i >= 0; i--)
        {
            // Find tracks with no attached source
            if (tracks[i].trackSource == null)
            {
                safeListOfTracksToRemove.Add(i);
            }
        }

        // Safely remove empty tracks
        foreach (int indexToRemove in safeListOfTracksToRemove)
        {
            tracks.RemoveAt(indexToRemove);
        }
    }


    private void FindDuplicateTracks()
    {
        // TODO: Find duplicates
        // TODO: Remove or warn?
    }

    /// <summary>
    /// Update displayed information for tracks
    /// </summary>
    public void UpdateInfo()
    {
        foreach (MusicTrack track in tracks)
        {
            track.UpdateEditorInfo();
        }
    }

    /// <summary>
    /// Initialize fade on all tracks
    /// </summary>
    public void FadeAll()
    {
        foreach (MusicTrack track in tracks)
        {
            BeginTrackFade(track);
        }
    }

    private void OnValidate()
    {
        foreach (MusicTrack track in tracks)
        {
            // Keep FadeDuration over 0
            if (track.FadeDuration < .0f)
            {
                track.FadeDuration = .0f;
            }

            // Clamp target volume if needed
            if (0 > track.TargetVolume || track.TargetVolume > 1)
            {
                track.TargetVolume = Mathf.Clamp01(track.TargetVolume);
            }
        }
    }






    /// <summary>
    /// Properties of music tracks
    /// </summary>
    [Serializable]
    public class MusicTrack
    {
        [SerializeField]
        public AudioSource trackSource;

        [ReadOnly] [SerializeField]
        AudioClip trackClip;

        [SerializeField]
        float targetVolume, fadeDuration = 0;

        /// <summary>Monitors how long the track has been fading to new TargetVolume</summary>
        float fadeTimer;
        /// <summary>Volume to fade from</summary>
        float fadeStartVolume;

        public MusicTrack(AudioSource track, AudioMixerGroup mixerGroup)
        {
            trackSource = track;

            // Set the AudioMixerGroup to use the music group
            track.outputAudioMixerGroup = mixerGroup;

            UpdateEditorInfo();
        }

        public void StartFade(float newTargetVolume, float newFadeDuration)
        {
            TargetVolume = newTargetVolume;
            FadeDuration = newFadeDuration;
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
                Debug.Log("Timer over: setting volume");
                Volume = TargetVolume;
                StopFadeTimer();
            }
            else
            {
                Debug.Log("Timer maxed for " + trackSource.gameObject.name);
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
            Debug.Log("Reset fadetimer");
            fadeStartVolume = Volume;
        }

        public void StopFadeTimer()
        {
            fadeTimer = float.MaxValue;
        }

        /// <summary>
        /// Update track information. Mostly used for ReadOnly info
        /// </summary>
        public void UpdateEditorInfo()
        {            
            // Clip info
            if (trackClip != trackSource.clip)
            {
                trackClip = trackSource.clip;
            }            
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