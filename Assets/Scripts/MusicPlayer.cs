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

    private void Start()
    {
        // Testing TODO: Remove testing
        BeginTrackFade(tracks[0], 0.8f);
    }

    /// <summary>
    /// Fades the music track to the desired volume
    /// </summary>
    /// <param name="track">Music track to fade</param>
    /// <param name="targetVolume">Desired volume</param>
    private void BeginTrackFade(MusicTrack track, float targetVolume = .0f)
    {
        track.TargetVolume = targetVolume;
        track.FadeTrackOverTime();
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
        //tracks.Remove(tracks[4]);
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
    /// Properties of music tracks
    /// </summary>
    [Serializable]
    public class MusicTrack
    {
        [SerializeField]
        public AudioSource trackSource;

        [SerializeField]
        float targetVolume, fadeDuration = 0;

        // Monitors how long the track has been fading to new TargetVolume
        float fadeTime;


        public MusicTrack(AudioSource track, AudioMixerGroup mixerGroup)
        {
            trackSource = track;

            // Set the AudioMixerGroup to use the music group
            track.outputAudioMixerGroup = mixerGroup;
        }

        public void StartFade(float newTargetVolume, float newFadeDuration)
        {

            FadeDuration = newFadeDuration;
        }

        /// <summary>
        /// Change track volume over time, duration set by FadeDuration
        /// </summary>
        public void FadeTrackOverTime()
        {
            if (fadeTime <= fadeDuration)
            {
                fadeTime += Time.unscaledDeltaTime;
                float lerpTarget = TargetVolume * (fadeTime / FadeDuration);
                Volume = Mathf.Lerp(Volume, lerpTarget, 0.5f);
            }
            Debug.Log("Volume: " + Volume);
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

        public float Volume
        {
            get
            {
                return trackSource.volume;
            }

            protected set
            {
                trackSource.volume = value;
            }
        }

        public float TargetVolume
        {
            get
            {
                return targetVolume;
            }

            set
            {
                // Reset fadeTime when a new target is set
                fadeTime = 0f;
                targetVolume = value;
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