using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{ // Ok Janne...

    [Tooltip("All the scenes that will be loaded.")]
    [SerializeField]
    int[] scenesBuildIndex;

    private void Awake()
    {
        LoadScenes();
    }

    /// <summary>
    /// Loads all scenes ontop of eachother and destroys the SceneController.
    /// </summary>
    private void LoadScenes()
    {
        foreach (int scene in scenesBuildIndex)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }

        Destroy(gameObject);
    }
}
