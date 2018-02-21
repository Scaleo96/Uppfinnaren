using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MusicMixer
{
    public class MusicPlayer : MonoBehaviour
    {
        /// <summary>
        /// Prefix used for debug messages displayer by the MusicPlayer.
        /// Accepts RichText formatting.
        /// </summary>
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

        [HideInInspector]
        public MusicComposition[] compositions;

        [Header("Music Tracks")]
        [SerializeField]
        private List<MusicTrack> tracks = new List<MusicTrack>();

        [Header("Batch handling of tracks")]
        [SerializeField]
        [Tooltip("Automatically add and generate music tracks from any viable child GameObjects?")]
        private bool autoPopulateTracks;

        [SerializeField]
        private bool automaticRenaming;

        [SerializeField]
        [Tooltip("What is the default fade duration of music tracks")]
        private float defaultFadeDuration;

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
            if (Instance != null)
            {
                if (Debug.isDebugBuild) LogWarning("Other instance of MusicPlayer already running. Terminating.");
                Destroy(gameObject);
            }
            else
            {
                if (Debug.isDebugBuild) Log("Initializing MusicPlayer", this);
                Instance = this;

                // Set MusicPlayer to be persistant
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            // Reset singleton
            if (Instance == this)
            {
                Instance = null;
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
        /// Starts playing track clip and begins fade
        /// </summary>
        /// <param name="trackToPlay"></param>
        /// <param name="fadeIn">Fade in to max volume or min volume?</param>
        /// <returns></returns>
        internal bool PlayTrack(MusicTrack trackToPlay, bool fadeIn)
        {
            float volume = fadeIn ? 1f : 0f;
            return PlayTrack(trackToPlay, volume);
        }

        /// <summary>
        /// Starts playing track clip and begins fade
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

                // Start playing track at volume 0 if it's not playing already
                if (!trackToPlay.trackSource.isPlaying)
                {
                    trackToPlay.trackSource.Play();
                    trackToPlay.trackSource.volume = 0f;
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

        /// <summary>
        /// Starts playing track clip and begins fade
        /// </summary>
        /// <param name="trackToPlay"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
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

        public void ActivateCompositionGroup(MusicComposition composition)
        {
            composition.ActivateGroup();
        }

        public void DeactivateCompositionGroup(MusicComposition composition)
        {
            composition.DeactivateGroup();
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

        public static MusicPlayer Instance
        {
            get
            {
                return instance;
            }

            private set
            {
                instance = value;
            }
        }
    }
}