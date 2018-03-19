using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempEndsceneLoader : MonoBehaviour {

    public void LoadScene(string namn)
    {
        SceneManager.LoadScene(namn, LoadSceneMode.Single);

    }
}
