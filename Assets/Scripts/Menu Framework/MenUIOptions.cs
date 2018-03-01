using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MenUI
{
    public class MenUIOptions : MonoBehaviour
    {
        private const string MASTER_VOL_PREF = "masterVol";
        private const string MUSIC_VOL_PREF = "musicVol";
        private const string SFX_VOL_PREF = "sfxVol";
        private const string AMBIENCE_VOL_PREF = "ambienceVol";

        /// <summary>
        /// Audio
        /// </summary>
        [Header("Audio Settings")]
        [SerializeField]
        private AudioMixer masterMixer;     // Master mixer to be used

        // References to UI objects
        [SerializeField]
        private Slider masterVolSlider;

        [SerializeField]
        private Slider musicVolSlider;

        [SerializeField]
        private Slider sfxVolSlider;

        [SerializeField]
        private Slider ambienceVolSlider;

        [SerializeField]
        private Toggle languageToggle;

        private static bool languageIsEnglish = true;

        private void Awake()
        {
            LoadPrefs();
        }

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
            SetMasterLevel(PlayerPrefs.GetFloat(MASTER_VOL_PREF));
            SetMusicLevel(PlayerPrefs.GetFloat(MUSIC_VOL_PREF));
            SetSoundFXLevel(PlayerPrefs.GetFloat(SFX_VOL_PREF));
            SetSoundFXLevel(PlayerPrefs.GetFloat(AMBIENCE_VOL_PREF));

            // Set slider values to correspond to the actual volume
            SetSliderValues(masterVolSlider, MASTER_VOL_PREF);
            SetSliderValues(musicVolSlider, MUSIC_VOL_PREF);
            SetSliderValues(sfxVolSlider, SFX_VOL_PREF);
            SetSliderValues(sfxVolSlider, AMBIENCE_VOL_PREF);

            // Load language settings
            SetLanguageToEnglish((PlayerPrefs.GetInt("isEnglish", 1) == 1 ? true : false));
            //languageToggle.isOn = languageIsEnglish;
        }

        /// <summary>
        /// Checks the volume of a given mixer group in the master mixer and adjusts the slider accordingly
        /// </summary>
        /// <param name="slider">Slider to set</param>
        /// <param name="mixerGroup">String name of the mixer group</param>
        private void SetSliderValues(Slider slider, string mixerGroup)
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
            SetAudioLevel(audioLevel, MASTER_VOL_PREF);
        }

        public void SetMusicLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, MUSIC_VOL_PREF);
        }

        public void SetSoundFXLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, SFX_VOL_PREF);
        }

        public void SetAmbienceLevel(float audioLevel)
        {
            SetAudioLevel(audioLevel, AMBIENCE_VOL_PREF);
        }

        public void SetLanguageToEnglish(bool isEnglish)
        {
            languageIsEnglish = isEnglish;
            PlayerPrefs.SetInt("isEnglish", (isEnglish ? 1 : 0));
            PlayerPrefs.Save();
        }
    }
}