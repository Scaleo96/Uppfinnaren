using UnityEngine;

namespace MenUI
{
    [RequireComponent(typeof(MenUI))]
    public class Pause : MonoBehaviour
    {

        MenUI menUI;

        [SerializeField]
        MenuParameters pauseMenuParameters;

        private bool isPaused = false;

        // Use this for initialization
        void Start()
        {
            if (menUI == null)
            {
                // Set reference for MenUI
                menUI = GetComponent<MenUI>();
            }

            // Set reference for PauseMenuPanel if blank
            if (pauseMenuParameters.menuGameObject == null)
                pauseMenuParameters.menuGameObject = transform.Find("PauseMenuPanel").gameObject;
        }

        // Update is called once per frame
        void LateUpdate() 
        {

            // If the "Cancel" input is used
            if (Input.GetButtonDown("Cancel"))
            {
                // If main menu isn't active and displayed
                if (!menUI.GetIsInMainMenu())
                {
                    if (isPaused && menUI.GetActiveMenu() == pauseMenuParameters.menuGameObject)
                    {
                        DoUnpause();
                    }
                    else if (menUI.GetActiveMenu() != null)
                    {
                        menUI.CloseMenu();
                    }
                    else if (!isPaused)
                    {
                        DoPause();
                    }
                }
            }

        }

        /// <summary>
        /// Unpause the game and hide the pause menu
        /// </summary>
        public void DoUnpause()
        {
            isPaused = false;
            if (Debug.isDebugBuild) Debug.Log("Unpaused", this);
            // Update time normally
            Time.timeScale = 1;
            menUI.CloseMenu();
        }

        /// <summary>
        /// Pause the game and show the pause menu
        /// </summary>
        void DoPause()
        {
            isPaused = true;
            if (Debug.isDebugBuild) Debug.Log("Paused", this);
            // Stop time update
            Time.timeScale = 0;
            menUI.OpenMenu(pauseMenuParameters);
        }

        public void BackToMainMenu()
        {
            DoUnpause();
            menUI.ReturnToMainMenu();
        }

        public void Restart()
        {
            DoUnpause();
            menUI.ReloadLevel();
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
        }
    }
}