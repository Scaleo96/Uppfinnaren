using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScreenController : MonoBehaviour
{
    GameObject[] puzzleScreens;

    private void Awake()
    {
        puzzleScreens = new GameObject[transform.childCount];

        // Puts all the children of the controller into the puzzleScreens array.
        for (int i = 0; i < puzzleScreens.Length; i++)
        {
            puzzleScreens[i] = transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Sets if the given screen is active or not.
    /// </summary>
    /// <param name="id">The id of the screen.</param>
    public void SetActiveScreen(int id, bool value)
    {
        puzzleScreens[id].SetActive(value);
        GameController.instance.SetActiveMovement(value);
    }
}
