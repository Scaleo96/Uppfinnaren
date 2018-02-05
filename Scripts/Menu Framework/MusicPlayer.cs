using UnityEngine;
using UnityEngine.Audio;

namespace MenUI
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MenUI))]
    [RequireComponent(typeof(GameOver))]
    public class MusicPlayer : MonoBehaviour
    {

        [SerializeField]
        AudioClip music;
        float basePitch;
        [SerializeField]
        float maxPitch = 3f;

        AudioSource musicPlayer;

        [Header("Audio snapshots")]
        [SerializeField]
        AudioMixerSnapshot menu;
        [SerializeField]
        AudioMixerSnapshot unpaused;
        [SerializeField]
        AudioMixerSnapshot gameOverSnapshot;

        [SerializeField]
        [Tooltip("In seconds")]
        float transitionTime = 2f;


        MenUI menUI;
        GameOver gameOverScript;

        // Use this for initialization
        void Start()
        {
            // Check if the audio source is using the correct music sample
            if (GetComponent<AudioSource>().clip != music)
            {
                // Set the audio clip to the designated music
                gameObject.GetComponent<AudioSource>().clip = music;
                gameObject.GetComponent<AudioSource>().Play();
            }

            menUI = GetComponent<MenUI>();
            gameOverScript = GetComponent<GameOver>();

            // Store base pitch
            basePitch = GetComponent<AudioSource>().pitch;

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (gameOverScript.GetGameOver() && gameOverSnapshot != null)
            {
                gameOverSnapshot.TransitionTo(transitionTime * Time.timeScale);
                SetPitch(basePitch);
            }
            else if (menUI.GetIsShowingMenu() && menu != null)
            {
                if (Time.timeScale <= 0f)
                {
                    menu.TransitionTo(0f);
                }
                else
                {
                    menu.TransitionTo(transitionTime * Time.timeScale);
                }

            }
            else if (unpaused != null)
            {
                unpaused.TransitionTo(transitionTime * Time.timeScale);
            }


        }

        public void SetPitch(float pitch = 1f)
        {
            GetComponent<AudioSource>().pitch = Mathf.Clamp(pitch, basePitch, maxPitch);
        }
    }
}