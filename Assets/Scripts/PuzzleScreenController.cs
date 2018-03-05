using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScreenController : MonoBehaviour
{
    GameObject[] puzzleScreens;

    List<int> deactivatedId = new List<int>();

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
        if (!deactivatedId.Contains(id))
        {
            GlobalStatics.PuzzleScreenOn = true;
            puzzleScreens[id].SetActive(true);
            GameController.instance.SetActiveMovement(false);
        }
    }

    /// <summary>
    /// Deactivates the given screen.
    /// </summary>
    /// <param name="id">The id of the screen.</param>
    public void DeactivateScreen(int id)
    {
        GlobalStatics.PuzzleScreenOn = false;
        puzzleScreens[id].SetActive(false);
        GameController.instance.SetActiveMovement(true);
    }

    public void PermenentDeactivate(int id)
    {
        deactivatedId.Add(id);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            for (int i = 0; i < puzzleScreens.Length; i++)
            {
                puzzleScreens[i].SetActive(false);
            }
            GameController.instance.SetActiveMovement(true);
            GlobalStatics.PuzzleScreenOn = false;
        }
    }
}
