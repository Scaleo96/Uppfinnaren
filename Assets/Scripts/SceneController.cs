using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{ // Ok Janne...
    [SerializeField]
    private Slider loadingBar;

    [Tooltip("All the scenes that will be loaded.")]
    [SerializeField]
    private int[] scenesBuildIndex;

    private List<AsyncOperation> finishedLoadingLevels = new List<AsyncOperation>();
    private Event theEvent;
    private bool primaryHasLoaded;

    private void Start()
    {
        LoadScenes();
    }

    /// <summary>
    /// Loads all scenes ontop of eachother and destroys the SceneController.
    /// </summary>
    private void LoadScenes()
    {
        StartCoroutine(LoadPrimarySceneInBackground(scenesBuildIndex[0]));

        for (int i = 1; i < scenesBuildIndex.Length; i++)
        {
            StartCoroutine(LoadSceneInBackground(scenesBuildIndex[i]));
        }

        StartCoroutine(WaitForScenesToFinishLoading());
    }

    private IEnumerator LoadSceneInBackground(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            // Check if the load has finished
            if (operation.progress >= 0.9f)
            {
                if (!finishedLoadingLevels.Contains(operation))
                {
                    finishedLoadingLevels.Add(operation);
                }
            }

            // Iterate one frame
            yield return null;
        }
    }

    private IEnumerator LoadPrimarySceneInBackground(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            // Unity only supplies the progress of the first 90% of the operation, let's have that reflected in the progress bar
            float progress = Mathf.Clamp01(operation.progress / .9f);

            // Update loading bar progress
            if (loadingBar)
            {
                loadingBar.value = progress;
            }

            // Check if the load has finished
            if (operation.progress >= 0.9f)
            {
                if (!finishedLoadingLevels.Contains(operation))
                {
                    finishedLoadingLevels.Add(operation);
                }
            }

            primaryHasLoaded = true;

            // Iterate one frame
            yield return null;
        }
    }

    private IEnumerator WaitForScenesToFinishLoading()
    {
        bool hasFinishedLoading = false;
        while (!hasFinishedLoading)
        {
            hasFinishedLoading = scenesBuildIndex.Length == finishedLoadingLevels.Count;

            if (primaryHasLoaded)
            {
                // Loading progress
                float loadingProgress = .0f;
                for (int i = 0; i < finishedLoadingLevels.Count; i++)
                {
                    float progress = finishedLoadingLevels[i].progress;
                    progress = progress / scenesBuildIndex.Length;

                    loadingProgress += progress;
                }

                // Update loading bar progress
                if (loadingBar)
                {
                    loadingBar.value = loadingProgress;
                }
                
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}