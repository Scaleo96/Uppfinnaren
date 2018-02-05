using UnityEngine;

namespace MenUI
{
    /// <summary>
    /// Handles termination of application
    /// </summary>
    public class QuitGame
    {

        /// <summary>
        /// Exits application in standalone mode or ends play mode in editor mode
        /// </summary>
        public static void Quit()
        {
            //If we are running in a standalone build of the game
#if UNITY_STANDALONE
            //Quit the application
            Application.Quit();
#endif

            //If we are running in the editor
#if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}