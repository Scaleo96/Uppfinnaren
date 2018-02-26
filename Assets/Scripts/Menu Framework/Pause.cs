using UnityEngine;
using UnityEngine.Audio;

namespace MenUI
{
    [RequireComponent(typeof(MenUI))]
    public class Pause : MonoBehaviour
    {
        private MenUI menUI;

        [SerializeField]
        private MenuParameters pauseMenuParameters;

        private bool isPaused = false;

        [Header("Audio snapshots")]
        [SerializeField]
        private float snapshotTransitionTime = 1f;

        [SerializeField]
        private AudioMixerSnapshot pauseSnapshot;

        [SerializeField]
        private AudioMixerSnapshot normalSnapshot;

        // Use this for initialization
        private void Start()
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
        private void LateUpdate()
        {
            HandleInput();
        }

        private void HandleInput()
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
            ChangeAudioSnapshot(normalSnapshot);
            menUI.CloseMenu();
        }

        private void ChangeAudioSnapshot(AudioMixerSnapshot snapshot)
        {
            // Requires mixer update mode to be in Unscaled Time, otherwise it will switch instantly if timescale is 0
            switch (snapshot.audioMixer.updateMode)
            {
                case AudioMixerUpdateMode.Normal:
                    if (Time.timeScale <= 0)
                    {
                        // Transition immediately if timescale is 0
                        snapshot.TransitionTo(0f);
                    }
                    else
                    {
                        snapshot.TransitionTo(snapshotTransitionTime * Time.timeScale);
                    }
                    break;

                case AudioMixerUpdateMode.UnscaledTime:
                    snapshot.TransitionTo(snapshotTransitionTime);
                    break;

                default:
                    snapshot.TransitionTo(snapshotTransitionTime);
                    break;
            }
        }

        /// <summary>
        /// Pause the game and show the pause menu
        /// </summary>
        private void DoPause()
        {
            isPaused = true;
            if (Debug.isDebugBuild) Debug.Log("Paused", this);
            // Stop time update
            Time.timeScale = 0;
            menUI.OpenMenu(pauseMenuParameters);

            ChangeAudioSnapshot(pauseSnapshot);
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