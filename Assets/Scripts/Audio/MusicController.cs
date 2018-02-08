using System.Collections;
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
                    // Check for and instantiate default MusicPlayer prefab in Resources folder
                    if (Resources.FindObjectsOfTypeAll(typeof(MusicPlayer)).Length > 0)
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
                    else // If no default resource can be found a new empty one will be instantiated instead to avoid null reference and a warning will be displayed
                    {
                        // Make a new gameobject and attach the music player to it
                        GameObject refObj = new GameObject("New Music Player object");
                        refObj.AddComponent<MusicPlayer>();
                        MusicPlayer newMusPla = refObj.GetComponent<MusicPlayer>();

                        // Warn
                        LogWarning("No MusicPlayer found in active scenes and default resource is missing! A new (empty) one was created, but you probably don't want this", refObj);
                        return newMusPla;
                    }
                }
            }
        }

        // TODO: Get all tracks
        public static List<MusicTrack> GetAllTracks()
        {
            return ActiveMusicPlayer.Tracks;
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

        // TODO: Fade specific track
        public static void FadeTrack(MusicTrack trackToFade, float targetVolume = 0f)
        {
            ActiveMusicPlayer.BeginTrackFade(trackToFade, targetVolume);
        }

        public static void FadeTrack(MusicTrack trackToFade, float targetVolume = 0f, float fadeDuration = 0f)
        {
            ActiveMusicPlayer.BeginTrackFade(trackToFade, targetVolume, fadeDuration);
        }

        public static void FadeTrack(int trackIndex, float targetVolume = 0f)
        {
            FadeTrack(GetAllTracks()[trackIndex], targetVolume);
        }

        public static void FadeTrack(int trackIndex, float targetVolume, float fadeDuration)
        {
            FadeTrack(GetAllTracks()[trackIndex], targetVolume, fadeDuration);
        }


        /// <summary>
        /// Logs a debug warning message with unified formatting for MusicController
        /// </summary>
        /// <param name="warningMessage">Warning message to send to the console</param>
        /// <param name="context">Defaults to the MusicPlayer</param>
        static void LogWarning(string warningMessage, UnityEngine.Object context = null)
        {
            Debug.LogWarning(debugPrefix + warningMessage, context);
        }
    }
}
