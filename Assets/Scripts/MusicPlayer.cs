using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour {

    [SerializeField]
    bool autoPopulateTracks;

    [SerializeField]
    List<MusicTrack> tracks;

    [SerializeField]
    AudioMixerGroup musicMixer;

    private void OnValidate()
    {
        // TODO: More fitting place than OnValidate?
        AutoPupulateTracksWithChildren();
    }

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
    private void AutoPupulateTracksWithChildren()
    {
        // TODO: Make sure preexisting tracks and their settings are removed/overriden
        if (autoPopulateTracks)
        {
            // Find children with audiosources (tracks)
            AudioSource[] tempTracks = transform.GetComponentsInChildren<AudioSource>();

            // Make new array with audiosources
            List<MusicTrack> tempTrackProperties = new List<MusicTrack>();
            foreach (AudioSource track in tempTracks)
            {
                tempTrackProperties.Add(new MusicTrack(track, musicMixer));
            }
            tracks = tempTrackProperties;
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
        /// Change music over time, duration set by FadeDuration
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