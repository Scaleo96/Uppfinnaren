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
        private AudioMixerGroup musicMixerBaseTracks;

        [SerializeField]
        [Tooltip("What mixer group the music for accompanying should play from (if different from musicMixer)")]
        private AudioMixerGroup musicMixerAccompanyingTracks;

        [Header("Pitch Shifter")]
        [SerializeField]
        private bool enablePitchShifter = false;

        [SerializeField]
        [Tooltip("What intervall should the Pitch Shifter change the pitch?")]
        private float pitchIntervall = 0.014f;

        [SerializeField]
        [Tooltip("Should pitch shifter change half an octave as well?")]
        private bool octaveShift = false;

        [HideInInspector]
        public MusicComposition[] compositions;

        [Header("Music Tracks")]
        [SerializeField]
        private bool startFadingIn;

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

        private MusicComposition activeMusicComposition;
        private int activeAccompanyingTrack;

        [SerializeField]
        [Tooltip("Will compositions wait for accompanying tracks to fade out completely before fading in new active tracks?")]
        private bool CompositionsWaitsForFadeOut = true;

        [SerializeField]
        [Range(0, 1f)]
        [Tooltip("Tolerance for minimum volume required before fading to next acvcompanying track")]
        private float compositionFadeTolerance = 0f;
        
        private void Awake()
        {
            CheckSingleton();
            StartFadingIn();
        }

        private void CheckSingleton()
        {
            // TODO: Fix bug where a MusicPlayer is allowed to do stuff during play before it has removed itself if there is another instance active.
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
                    if (musicMixerBaseTracks != null)
                    {
                        // Create a new MusicTrack with the unique source
                        tracks.Add(new MusicTrack(source, musicMixerBaseTracks));
                    }
                    else
                    {
                        tracks.Add(new MusicTrack(source));
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

        internal void StopTrack(MusicTrack track)
        {
            track.trackSource.Stop();
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

        public void ActivateCompositionGroup(MusicComposition activatingComposition)
        {
            if (activeMusicComposition != null)
            {
                bool compositionIsAlreadyActive = activatingComposition == activeMusicComposition;
                if (compositionIsAlreadyActive)
                {
                    LogWarning("Composition <i>" + activatingComposition.ToString() + "</i> is already active, aborting activation.");
                    return;
                }
                else
                {
                    activeMusicComposition.DeactivateComposition();
                }
            }

            activeMusicComposition = activatingComposition;
            activeMusicComposition.ActivateGroup(ActiveAccompanyingTrack);

            if (enablePitchShifter)
            {
                InvokePitchShifter();
            }            
        }

        public void DeactivateCompositionGroup(MusicComposition composition)
        {
            composition.DeactivateComposition();
        }

        private void InvokePitchShifter()
        {
            Log("Invoking PitchShifter");
            CancelInvoke("PitchShifter");
            InvokeRepeating("PitchShifter", ActiveBaseTrackClip.length, ActiveBaseTrackClip.length);
        }

        private void PitchShifter()
        {
            if (musicMixerAccompanyingTracks == null)
            {
                LogWarning("No mixer group set for accompanying tracks. Aborting pitch shifter.");
                return;
            }

            float randomPitch = 1f;

            if (octaveShift)
            {
                float halfDown = 0.75f;
                float flat = 1f;
                float halfUp = 1.50f;

                int randomOctaveSelector = UnityEngine.Random.Range(0, 4);

                switch (randomOctaveSelector)
                {
                    case 0:
                        randomPitch = halfDown;
                        break;

                    case 1:
                        randomPitch = halfUp;
                        break;

                    default:
                        randomPitch = flat;
                        break;
                }
            }

            float pitchShift = UnityEngine.Random.Range(-pitchIntervall, pitchIntervall);

            randomPitch += pitchShift;

            musicMixerAccompanyingTracks.audioMixer.SetFloat("compTrackPitch", randomPitch);

            Log("Pitch set to: " + randomPitch, this);
        }

        /// <summary>
        /// Set mixer group based on wether it is a base track or accompanying track
        /// </summary>
        /// <param name="track">Track to change</param>
        /// <param name="isBaseTrack">Music tracks are usually set to base track music group unless otherwise specified</param>
        public void SetMixerGroup(MusicTrack track, bool isBaseTrack)
        {
            AudioMixerGroup currentMixerGroup = track.trackSource.outputAudioMixerGroup;
            AudioMixerGroup desiredMixerGroup;
            if (!isBaseTrack)
            {
                desiredMixerGroup = musicMixerAccompanyingTracks;
            }
            else
            {
                desiredMixerGroup = musicMixerBaseTracks;
            }

            if (currentMixerGroup != desiredMixerGroup)
            {
                track.trackSource.outputAudioMixerGroup = desiredMixerGroup;
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

        public int ActiveAccompanyingTrack
        {
            get
            {
                return activeAccompanyingTrack;
            }

            set
            {
                activeAccompanyingTrack = value;
                if (activeMusicComposition != null)
                {
                    activeMusicComposition.FadeToTrackExlusive(activeAccompanyingTrack);
                }
            }
        }

        public bool CompWaitForFade
        {
            get
            {
                return CompositionsWaitsForFadeOut;
            }
        }

        public float CompositionFadeTolerance
        {
            get
            {
                return compositionFadeTolerance;
            }
        }

        private AudioClip ActiveBaseTrackClip
        {
            get
            {
                return tracks[activeMusicComposition.baseTrackIndex].trackSource.clip;
            }
        }
    }
}