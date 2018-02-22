using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{ // Ok Janne...

    [Tooltip("All the scenes that will be loaded.")]
    [SerializeField]
    Scene[] scenes;

    private void Awake()
    {
        LoadScenes();
    }

    /// <summary>
    /// Loads all scenes ontop of eachother and destroys the SceneController.
    /// </summary>
    private void LoadScenes()
    {
        foreach (Scene scene in scenes)
        {
            SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Additive);
        }

        Destroy(gameObject);
    }
}
