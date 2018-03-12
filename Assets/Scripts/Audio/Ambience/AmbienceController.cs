using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicMixer;

namespace AmbienceMixer
{
    [System.Serializable]
    public struct AmbienceArea
    {
        public string name;
        public BoxCollider2D area;
        public AudioClip[] ambienceClips;
        public UnityEngine.Audio.AudioMixerGroup mixerGroup;

        [HideInInspector]
        public List<MusicTrack> ambienceTracks;
    }

    public class AmbienceController : MonoBehaviour
    {
        [SerializeField]
        AmbienceArea[] ambienceAreas;

        List<int> currentAmbienceAreas;

        Camera mainCamera;

        private void Awake()
        {
            currentAmbienceAreas = new List<int>();
        }

		private void Start()
		{
			mainCamera = Camera.main;
		}

        private void Update()
        {
            CheckCurrentAmbienceAreas();
            AmbienceAreaCheck();
        }

        /// <summary>
        /// Checks if a Vector2 point is within the given bounds.
        /// </summary>
        private bool IsInBounds(Vector2 pos, Bounds bounds)
        {
            if 
            (
                pos.x > bounds.min.x &&
                pos.x < bounds.max.x &&
                pos.y > bounds.min.y &&
                pos.y < bounds.max.y
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the camera is in the current ambience area and if not: removes the ambience noice for that area.
        /// </summary>
        private void CheckCurrentAmbienceAreas()
        {
            for (int i = 0; i < currentAmbienceAreas.Count; i++)
            {
                if (!IsInBounds(mainCamera.transform.position, ambienceAreas[currentAmbienceAreas[i]].area.bounds))
                {
                    if (ambienceAreas[currentAmbienceAreas[i]].ambienceTracks[0] != null)
                    {
                        if (ambienceAreas[currentAmbienceAreas[i]].ambienceTracks[0].trackSource.volume == ambienceAreas[currentAmbienceAreas[i]].ambienceTracks[0].TargetVolume)
                        {
                            Debug.Log("outside");
                            SetFadeOutAmbienceTracks(ambienceAreas[currentAmbienceAreas[i]], 2f);
                        }
                    }
                }

                foreach (MusicTrack ambienceTrack in ambienceAreas[currentAmbienceAreas[i]].ambienceTracks)
                {
                    ambienceTrack.FadeTrackOverTime();

                    if (ambienceTrack.trackSource.volume <= 0)
                    {
                        Destroy(ambienceTrack.trackSource);
                        currentAmbienceAreas.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Goes through each AmbienceArea and checks if the main camera is within it. Then starts the ambience noise specified for that area.
        /// </summary>
        private void AmbienceAreaCheck()
        {
            // TODO: Add check for already existing audios
            for (int i = 0; i < ambienceAreas.Length; i++)
            {
                if (IsInBounds(mainCamera.transform.position, ambienceAreas[i].area.bounds))
                {
                    if (!currentAmbienceAreas.Contains(i))
                    {
                        Debug.Log(GameController.instance.GetCurrentCharacter().EntityName + " (" + mainCamera.gameObject.name + ")" +
                            " entered <b>" + ambienceAreas[i].name + "</b> ambience area");

                        SpawnAudioSources(ambienceAreas[i], 2f);
                        currentAmbienceAreas.Add(i);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the AudioSource componenets on the AmbienceController gameObject.
        /// </summary>
        private void SetFadeOutAmbienceTracks(AmbienceArea ambienceArea, float fadeDuration)
        {
            Debug.Log("<b>Fading AudioTracks for:</b> " + ambienceArea.name);

            foreach (MusicTrack musicTrack in ambienceArea.ambienceTracks)
            {
                Debug.Log("<color=red>Fading</color> " + musicTrack + " and " + musicTrack.trackSource);
                musicTrack.StartFade(0f, fadeDuration);
            }        
        }

        /// <summary>
        /// Creates the given count of AudioSources (assigns audioClips) on the AmbienceController gameObject.
        /// </summary>
        private void SpawnAudioSources(AmbienceArea ambienceArea, float fadeDuration)
        {
            ambienceArea.ambienceTracks.Clear();

            for (int i = 0; i < ambienceArea.ambienceClips.Length; i++)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = ambienceArea.ambienceClips[i];

                ambienceArea.ambienceTracks.Add(
                    new MusicTrack
                    (
                        audioSource,
                        ambienceArea.mixerGroup
                    )
                );

                // Debug.Log(ambienceArea.ambienceTracks[i]);

                ambienceArea.ambienceTracks[i].trackSource.volume = 0f;
                ambienceArea.ambienceTracks[i].StartFade(1f, fadeDuration);

                audioSource.Play();
            }           
        }
    }
}
