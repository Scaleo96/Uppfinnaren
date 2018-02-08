using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MusicMixer
{
    public class MusicPlayer : MonoBehaviour
    {
        private const string DEBUG_PREFIX = "<color=darkblue><b>MusicPlayer</b></color> - ";

        // Used for singleton
        private static MusicPlayer instance;

        [SerializeField]
        [Tooltip("Displays debug messages in console")]
        private bool verbose = true;

        [SerializeField]
        [Tooltip("What mixer group the music should play from")]
        private AudioMixerGroup musicMixer;

        [SerializeField]
        private bool startFadingIn;

        [Header("Music Tracks")]
        [SerializeField]
        private List<MusicTrack> tracks = new List<MusicTrack>();

        [Header("Batch handling of tracks")]
        [SerializeField]
        private bool autoPopulateTracks;
        [SerializeField]
        private bool automaticRenaming;

        [SerializeField]
        float defaultFadeDuration;

        [Tooltip("Attempt to remove (UnityEngine.AudioClip) appended at the end of renamed children?")]
        private bool trunctateNameOfRenamedTracks = true;

        private void Awake()
        {
            CheckSingleton();
            StartFadingIn();
        }

        private void CheckSingleton()
        {
            // Make sure there are no other running instances of the MenUI
            if (instance != null)
            {
                if (Debug.isDebugBuild) LogWarning("Other instance of MusicPlayer already running. Terminating.");
                Destroy(gameObject);
            }
            else
            {
                if (Debug.isDebugBuild) Log("Initializing MusicPlayer", this);
                instance = this;

                // Set MusicPlayer to be persistant
                DontDestroyOnLoad(gameObject);
            }
        }

        private void StartFadingIn()
        {
            if (!startFadingIn)
            {
                foreach (MusicTrack track in tracks)
                {
                    track.StopFadeTimer();
                }
            }
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


        /// <summary>
        /// Fades the music track to the desired volume
        /// </summary>
        /// <param name="track">Music track to fade</param>
        /// <param name="targetVolume">Desired volume</param>
        private void BeginTrackFade(MusicTrack track, float targetVolume = .0f)
        {
            track.TargetVolume = targetVolume;
            track.StartFadeTimer();
        }

        /// <summary>
        /// Fades the music track to the desired volume
        /// </summary>
        /// <param name="track">Music track to fade</param>
        /// <param name="targetVolume">Desired volume</param>
        /// <param name="fadeDuration">New fade duration</param>
        public void BeginTrackFade(MusicTrack track, float targetVolume = .0f, float fadeDuration = .0f)
        {
            track.FadeDuration = fadeDuration;
            BeginTrackFade(track, targetVolume);
        }

        /// <summary>
        /// Fades the music track to the previously set target volume
        /// </summary>
        /// <param name="track">Music track to fade</param>
        public void BeginTrackFade(MusicTrack track)
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
                    if (musicMixer != null)
                    {
                        // Create a new MusicTrack with the unique source
                        tracks.Add(new MusicTrack(source, musicMixer));
                    }
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
        /// Starts playing track clip and fade in
        /// </summary>
        /// <param name="trackToPlay"></param>
        /// <param name="volume"></param>
        /// <param name="newFadeDuration"></param>
        /// <returns></returns>
        internal bool PlayTrack(MusicTrack trackToPlay, float volume, float newFadeDuration)
        {
            if (trackToPlay.trackSource != null)
            {
                if (trackToPlay.trackSource.clip == null)
                {
                    // Failed: No clip to play
                    LogWarning("Unable to play track, clip missing: " + trackToPlay.trackSource);
                    return false;
                }

                // Start playing track if it's nto playing
                if (!trackToPlay.trackSource.isPlaying)
                {
                    trackToPlay.trackSource.Play();
                }

                // Initialize fade
                trackToPlay.StartFade(volume, newFadeDuration);

                if (trackToPlay.trackSource.isPlaying)
                {
                    // Success
                    return true;
                }
                else
                {
                    // Failed: Track not playing
                    LogWarning("Failed playing track.", trackToPlay.trackSource);
                    return false;
                }
            }
            else
            {
                // Failed: Unable to find AudioSource
                LogWarning("Unable to find AudioSource");
                return false;
            }
        }

        internal bool PlayTrack(MusicTrack trackToPlay, float volume = 1f)
        {
            return PlayTrack(trackToPlay, volume, trackToPlay.FadeDuration);
        }

        internal void StopTrack()
        {
            throw new NotImplementedException();
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
        /// Renames the game object to match the source file of the AudioSource
        /// </summary>
        public void RenameObjectsToSourceFile()
        {
            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.GetComponent<AudioSource>())
                {
                    string newName = child.GetComponent<AudioSource>().clip.ToString();
                    if (trunctateNameOfRenamedTracks)
                    {
                        newName = newName.Replace(" (UnityEngine.AudioClip)", "");
                    }
                    child.name = newName;
                }
            }
        }

        public void SetDefaultFadeDurationOnAllTracks()
        {
            foreach (MusicTrack track in tracks)
            {
                track.FadeDuration = defaultFadeDuration;
            }
        }

        /// <summary>
        /// Logs a debug warning message with unified formatting for MusicPlayer
        /// </summary>
        /// <param name="warningMessage">Warning message to send to the console</param>
        /// <param name="context">Defaults to the MusicPlayer</param>
        private void LogWarning(string warningMessage, UnityEngine.Object context = null)
        {
            if (verbose)
            {
                // Default context to MusicPlayer calling it
                if (context == null)
                {
                    context = this;
                }

                Debug.LogWarning(DEBUG_PREFIX + warningMessage, context);
            }
        }

        /// <summary>
        /// Logs a debug warning message with unified formatting for MusicPlayer
        /// </summary>
        /// <param name="warningMessage">Warning message to send to the console</param>
        /// <param name="context">Defaults to the MusicPlayer</param>
        private void Log(string warningMessage, UnityEngine.Object context = null)
        {
            if (verbose)
            {
                // Default context to MusicPlayer calling it
                if (context == null)
                {
                    context = this;
                }

                Debug.Log(DEBUG_PREFIX + warningMessage, context);
            }
        }

        public List<MusicTrack> Tracks
        {
            get
            {
                return tracks;
            }
        }

        public bool AutoPopulateTracks
        {
            get
            {
                return autoPopulateTracks;
            }
        }

        public bool AutomaticRenaming
        {
            get
            {
                return automaticRenaming;
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

        [ReadOnly]
        [SerializeField]
        private AudioClip trackClip;

        [ReadOnly]
        [SerializeField]
        private float currentVolume;

        [SerializeField] [Range(0, 1)]
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
            Debug.Log("Reset fadetimer");
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