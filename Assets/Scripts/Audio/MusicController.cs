using System.Collections.Generic;
using UnityEngine;

namespace MusicMixer
{
    static public class MusicController
    {
        /// <summary>
        /// Prefix used for all logged debug messages
        /// </summary>
        private static readonly string debugPrefix = "<color=darkblue><b>MusicController</b></color> - ";

        /// <summary>
        /// Finds and returns the active MusicPlayer.
        /// If there are no MusicPlayer present a new one will be instantiated from default if possible, empty if resource is missing.
        /// </summary>
        private static MusicPlayer ActiveMusicPlayer
        {
            get
            {
                // Try to find MusicPlayer in scene
                if (GameObject.FindObjectOfType<MusicPlayer>() != null)
                {
                    return GameObject.FindObjectOfType<MusicPlayer>();
                }
                else
                {
                    return SpawnMusicPlayer();
                }
            }
        }

        private static MusicPlayer SpawnMusicPlayer()
        {
            // Check for and instantiate default MusicPlayer prefab in Resources folder
            if (Resources.FindObjectsOfTypeAll(typeof(MusicPlayer)).Length > 0)
            {
                return SpawnDefaultMusicPlayer();
            }
            else
            {
                // If no default resource can be found a new empty one will be instantiated instead to avoid null reference and a warning will be displayed
                return SpawnEmptyMusicPlayer();
            }
        }

        private static MusicPlayer SpawnEmptyMusicPlayer()
        {
            // Make a new gameobject and attach the music player to it
            GameObject refObj = new GameObject("New Music Player object");
            refObj.AddComponent<MusicPlayer>();
            MusicPlayer newMusPla = refObj.GetComponent<MusicPlayer>();

            // Warn
            LogWarning("No MusicPlayer found in active scenes and default resource is missing! A new (empty) one was created, but you probably don't want this", refObj);
            return newMusPla;
        }

        /// <summary>
        /// Spawns default MusicPlayer from prefab in Resources
        /// </summary>
        /// <returns>Default MusicPlayer</returns>
        private static MusicPlayer SpawnDefaultMusicPlayer()
        {
            LogWarning("No MusicPlayer found in active scene. Instantiating default MusicPlayer located in Resources folder.");

            // Fetch/load the first MusicPlayer in the Resources folder
            MusicPlayer[] musicPlayerResourceArray = (MusicPlayer[])Resources.FindObjectsOfTypeAll(typeof(MusicPlayer));
            MusicPlayer defaultMusicPlayer = musicPlayerResourceArray[0];

            // Check and warn if there are more than one MusicPlayer in the Resources folder (we don't control which one is loaded)
            if (musicPlayerResourceArray.Length > 1)
            {
                LogWarning("Multiple MusicPlayer located in the Resource folder. Only the first found one will be used, remove the ones you do not wish to use.");
            }

            // Instantiate clone of MusicPlayer prefab
            GameObject newMPGameObj = GameObject.Instantiate(defaultMusicPlayer.gameObject);
            newMPGameObj.name = "Default Music Player";

            // Find and return the MusicPlayer
            MusicPlayer newMusicPlayer = newMPGameObj.GetComponent<MusicPlayer>();
            return newMusicPlayer;
        }

        /// <summary>
        /// Get all MusicTrack available in the current active MusicPlayer.
        /// </summary>
        /// <returns>List with all MusicTrack</returns>
        public static List<MusicTrack> GetAllTracks()
        {
            return ActiveMusicPlayer.Tracks;
        }

        /// <summary>
        /// Get all MusicTracks as strings. Has the same index as GetAllTracks and can be used to find index for a specific track.
        /// </summary>
        /// <returns>Array of all clips in active MusicPlayer</returns>
        public static string[] GetAllTracksAsString()
        {
            List<string> trackStrings = new List<string>();
            foreach (MusicTrack track in MusicController.GetAllTracks())
            {
                trackStrings.Add(track.ToString());
            }
            return trackStrings.ToArray();
        }

        /// <summary>
        /// Get all MusicTracks as strings. Has the same index as GetAllTracks and can be used to find index for a specific track.
        /// </summary>
        /// <returns>Array of all clips in active MusicPlayer</returns>
        public static string[] GetAllCompositionsAsString()
        {
            List<string> compositionStrings = new List<string>();
            foreach (MusicComposition composition in Compositions)
            {
                compositionStrings.Add(composition.ToString());
            }
            return compositionStrings.ToArray();
        }

        // Play track
        /// <summary>
        /// Attempts to play the supplied track
        /// </summary>
        /// <param name="trackToPlay">Reference to the track to play</param>
        /// <returns>Was the attempt successful?</returns>
        public static bool PlayTrack(MusicTrack trackToPlay)
        {
            bool success = ActiveMusicPlayer.PlayTrack(trackToPlay);
            if (!success)
            {
                LogWarning("Unable to play track");
            }
            return success;
        }

        // Stop track
        public static void StopTrack()
        {
            ActiveMusicPlayer.StopTrack();
        }

        /// <summary>
        /// Begins a crossfade of the specified track
        /// </summary>
        /// <param name="trackToFade">Which track should be crossfaded</param>
        /// <param name="targetVolume">What volume the track should end up with</param>
        /// <returns>Name of the track faded</returns>
        public static string FadeTrack(MusicTrack trackToFade, float targetVolume = 0f)
        {
            ActiveMusicPlayer.BeginTrackFade(trackToFade, targetVolume);
            return trackToFade.ToString();
        }

        /// <summary>
        /// Begins a crossfade of the specified track
        /// </summary>
        /// <param name="trackToFade">Which track should be crossfaded</param>
        /// <param name="targetVolume">What volume the track should end up with</param>
        /// <param name="fadeDuration">Over how many seconds the fade will travel to the target volume</param>
        /// <returns>Name of the track faded</returns>
        public static string FadeTrack(MusicTrack trackToFade, float targetVolume = 0f, float fadeDuration = 0f)
        {
            ActiveMusicPlayer.BeginTrackFade(trackToFade, targetVolume, fadeDuration);
            return trackToFade.ToString();
        }

        /// <summary>
        /// Begins a crossfade of the specified track using it's index in the active MusicPlayer
        /// </summary>
        /// <param name="trackToFade">Which track should be crossfaded</param>
        /// <param name="targetVolume">What volume the track should end up with</param>
        /// <param name="fadeDuration">Over how many seconds the fade will travel to the target volume</param>
        /// <returns>Name of the track faded</returns>
        public static string FadeTrack(int trackToFade, float targetVolume = 0f)
        {
            // TODO: Resolve missing index
            return FadeTrack(GetAllTracks()[trackToFade], targetVolume);
        }

        /// <summary>
        /// Begins a crossfade of the specified track using it's index in the active MusicPlayer
        /// </summary>
        /// <param name="trackToFade">Which track should be crossfaded</param>
        /// <param name="targetVolume">What volume the track should end up with</param>
        /// <param name="fadeDuration">Over how many seconds the fade will travel to the target volume</param>
        /// <returns>Name of the track faded</returns>
        public static string FadeTrack(int trackToFade, float targetVolume, float fadeDuration)
        {
            // TODO: Resolve missing index
            return FadeTrack(GetAllTracks()[trackToFade], targetVolume, fadeDuration);
        }

        /// <summary>
        /// Logs a debug warning message with unified formatting for MusicController
        /// </summary>
        /// <param name="warningMessage">Warning message to send to the console</param>
        /// <param name="context">Defaults to the MusicPlayer</param>
        private static void LogWarning(string warningMessage, UnityEngine.Object context = null)
        {
            Debug.LogWarning(debugPrefix + warningMessage, context);
        }

        public static MusicComposition[] Compositions
        {
            get
            {
                return ActiveMusicPlayer.compositions;
            }
        }

        public static string ActivateMusicComposition(MusicComposition composition)
        {
            ActiveMusicPlayer.ActivateCompositionGroup(composition);
            return composition.ToString();
        }

        public static void ActivateAccompanyingTrackExclusive(int compIndex)
        {
            ActiveMusicPlayer.ActiveAccompanyingTrack = compIndex;
        }

        public static void DeactivateAllCompositions()
        {
            foreach (MusicComposition composition in ActiveMusicPlayer.compositions)
            {
                composition.DeactivateComposition();
            }
        }
    }
}