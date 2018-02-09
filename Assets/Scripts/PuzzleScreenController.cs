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
    /// Activates the given screen.
    /// </summary>
    /// <param name="id">The id of the screen.</param>
    public void ActivateScreen(int id)
    {
        puzzleScreens[id].SetActive(true);
        GameController.instance.SetActiveMovement(false);
    }

    /// <summary>
    /// Deactivates the given screen.
    /// </summary>
    /// <param name="id">The id of the screen.</param>
    public void DeactivateScreen(int id)
    {
        puzzleScreens[id].SetActive(false);
        GameController.instance.SetActiveMovement(true);
    }
}
