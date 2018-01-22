using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MenUI
{
    public class MenUIOptions : MonoBehaviour
    {

        private void Awake()
        {
            LoadPrefs();
        }

        /// <summary>
        /// Audio
        /// </summary>
        [Header("Audio Settings")]

        [SerializeField]
        AudioMixer masterMixer;     // Master mixer to be used

        // References to UI objects
        [SerializeField]
        Slider masterVolSlider;
        [SerializeField]
        Slider musicVolSlider;
        [SerializeField]
        Slider sfxVolSlider;
        [SerializeField]
        Toggle SubtitlesToggle;

        static bool subtitlesEnabled = true;

        /// <summary>
        /// Adjusts specified audio groups volume
        /// </summary>
        /// <param name="audioLevel">Volume (DB)</param>
        /// <param name="audioGroup">Mixer group</param>
        private void SetAudioLevel(float audioLevel, string audioGroup)
        {
            masterMixer.SetFloat(audioGroup, audioLevel);
            PlayerPrefs.SetFloat(audioGroup, audioLevel);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load player preferences and restores them
        /// </summary>
        private void LoadPrefs()
        {
            // Load volume levels from PlayerPrefs and set volume levels in the mixer
            SetMasterLevel(PlayerPrefs.GetFloat("masterVol"));
            SetMusicLevel(PlayerPrefs.GetFloat("musicVol"));
            SetSoundFXLevel(PlayerPrefs.GetFloat("sfxVol"));

            // Set slider values to correspond to the actual volume
            SetSliderValues(masterVolSlider, "masterVol");
            SetSliderValues(musicVolSlider, "musicVol");
            SetSliderValues(sfxVolSlider, "sfxVol");

            // Load subtitles settings
            SubtitlesEnabled((PlayerPrefs.GetInt("subtitles", 1) == 1 ? true : false));
            SubtitlesToggle.isOn = subtitlesEnabled;
        }

        /// <summary>
        /// Checks the volume of a given mixer group in the master mixer and adjusts the slider accordingly
        /// </summary>
        /// <param name="slider">Slider to set</param>
        /// <param name="mixerGroup">String name of the mixer group</param>
        void SetSliderValues(Slider slider, string mixerGroup)
        {
            float volume;
            if (masterMixer.GetFloat(mixerGroup, out volume))
            {
                slider.value = volume;
            }
            else
            {
                if (Debug.isDebugBuild) Debug.LogWarning("Unable to change slider value - Incompatible mixer group key", slider);
            }
        }

        public void SetMasterLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, "masterVol");
        }

        public void SetMusicLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, "musicVol");
        }

        public void SetSoundFXLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, "sfxVol");
        }

        public void SubtitlesEnabled(bool enableSubtitles)
        {
            subtitlesEnabled = enableSubtitles;
            PlayerPrefs.SetInt("subtitles", (enableSubtitles ? 1 : 0));
            PlayerPrefs.Save();
        }
    }
}