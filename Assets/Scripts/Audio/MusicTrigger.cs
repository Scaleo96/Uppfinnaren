﻿using System.Collections;
using UnityEngine;

namespace MusicMixer
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MusicTrigger : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public int selectedComposition = 0;

        // Shared layer for camera and triggers. Invasive
        private int gameObjectLayer = 13;

        [SerializeField]
        [Tooltip("Optional delay for when to trigger the change")]
        private float startDelay;

        private void Start()
        {
            AddTriggerToCamera();
            InitializeComponents();
            SetLayer();
        }

        /// <summary>
        /// Checks if the main camera has a viable trigger. Converts a 2D collider into a trigger or adds a new one as needed.
        /// </summary>
        private void AddTriggerToCamera()
        {
            bool cameraHasCollider = Camera.main.GetComponent<Collider2D>();
            if (cameraHasCollider)
            {
                bool cameraColliderIsTrigger = Camera.main.GetComponent<Collider2D>().isTrigger;
                if (cameraColliderIsTrigger)
                {
                    return;
                }
                else
                {
                    Debug.Log("Changed the main camera collider into a trigger", gameObject);
                    Camera.main.GetComponent<Collider2D>().isTrigger = true;
                    return;
                }
            }
            else
            {
                // Add collider and make it a trigger
                Camera.main.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
            }

            // Make camera ignore raycast
            Camera.main.gameObject.layer = gameObjectLayer;
        }

        /// <summary>
        /// Makes sure the gameobject components are set correctly
        /// </summary>
        public void InitializeComponents()
        {
            if (!GetComponent<Rigidbody2D>().isKinematic)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
            }

            if (!GetComponent<Collider2D>().isTrigger)
            {
                GetComponent<Collider2D>().isTrigger = true;
                Debug.LogWarning("<b>MusicTrigger</b> needs the collider to be a trigger to work.", this);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("MainCamera"))
            {
                StartCoroutine(PrepareToChangeMusic(startDelay));
            }
        }

        private void ChangeMusic()
        {
            MusicController.ActivateMusicComposition(MusicController.Compositions[selectedComposition]);
        }

        private void OnValidate()
        {
            InitializeComponents();
        }

        private void SetLayer()
        {
            if (gameObject.layer != gameObjectLayer)
            {
                gameObject.layer = gameObjectLayer;
            }
        }

        private IEnumerator PrepareToChangeMusic(float delay = 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
            ChangeMusic();
        }
    }
}