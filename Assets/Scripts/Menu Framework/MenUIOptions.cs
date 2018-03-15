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

        // Default volume used on first run of the game
        private const float DEFAULT_VOLUME          = 0.7f;
        private const float DEFAULT_VOLUME_MUSIC    = 0.7f;
        private const float DEFAULT_VOLUME_SFX      = 0.5f;
        private const float DEFAULT_VOLUME_AMBIENCE = 0.2f;

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
            // TODO: Better math?
            float logFloat = audioLevel * 4000f;
            logFloat = -80f + Mathf.Log(logFloat) * 10f;
            logFloat = Mathf.Clamp(logFloat, -80f, 10f);

            masterMixer.SetFloat(audioGroup, logFloat);
            PlayerPrefs.SetFloat(audioGroup, audioLevel);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load player preferences and restores them
        /// </summary>
        private void LoadPrefs()
        {
            // Load volume levels from PlayerPrefs and set volume levels in the mixer
            SetMasterLevel(PlayerPrefs.GetFloat(MASTER_VOL_PREF, DEFAULT_VOLUME));
            SetMusicLevel(PlayerPrefs.GetFloat(MUSIC_VOL_PREF, DEFAULT_VOLUME_MUSIC));
            SetSoundFXLevel(PlayerPrefs.GetFloat(SFX_VOL_PREF, DEFAULT_VOLUME_SFX));
            SetSoundFXLevel(PlayerPrefs.GetFloat(AMBIENCE_VOL_PREF, DEFAULT_VOLUME_AMBIENCE));

            // Set slider values to correspond to the actual volume
            SetSliderValues(masterVolSlider, MASTER_VOL_PREF);
            SetSliderValues(musicVolSlider, MUSIC_VOL_PREF);
            SetSliderValues(sfxVolSlider, SFX_VOL_PREF);
            SetSliderValues(ambienceVolSlider, AMBIENCE_VOL_PREF);

            // Load language settings
            SetLanguageToEnglish((PlayerPrefs.GetInt("isEnglish", 1) == 1 ? true : false));
            //languageToggle.isOn = languageIsEnglish;
        }

        /// <summary>
        /// Checks the volume of a given mixer group in the master mixer and adjusts the slider accordingly
        /// </summary>
        /// <param name="slider">Slider to set</param>
        /// <param name="mixerGroup">String name of the mixer group</param>
        private void SetSliderValues(Slider slider, string preferenceString)
        {
            slider.value = PlayerPrefs.GetFloat(preferenceString);
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
            PlayerPrefs.SetInt("isEnglish", (isEnglish ? 1 : 0));
            PlayerPrefs.Save();
        }
    }
}