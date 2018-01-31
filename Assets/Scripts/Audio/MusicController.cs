using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicMixer
{
    static public class MusicController
    {

        public static MusicPlayer ActiveMusicPlayer
        {
            get
            {
                if (GameObject.FindObjectOfType<MusicPlayer>() != null)
                {
                    return GameObject.FindObjectOfType<MusicPlayer>();
                }
                else
                {
                    // TODO: Add Resource for the prefab containing the finished music player

                    // Make a new gameobject and attach the music player to it
                    GameObject refObj = new GameObject("New Music Player object");
                    refObj.AddComponent<MusicPlayer>();
                    MusicPlayer newMusPla = refObj.GetComponent<MusicPlayer>();

                    // Warn
                    LogWarning("No MusicPlayer found in active scenes! A new (empty) one was created, but you probably don't want this", refObj);
                    return newMusPla;
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


        /// <summary>
        /// Logs a debug warning message with unified formatting for MusicController
        /// </summary>
        /// <param name="warningMessage">Warning message to send to the console</param>
        /// <param name="context">Defaults to the MusicPlayer</param>
        static void LogWarning(string warningMessage, UnityEngine.Object context = null)
        {
            Debug.LogWarning("<color=darkblue><b>MusicController</b></color> - " + warningMessage, context);
        }
    }
}
