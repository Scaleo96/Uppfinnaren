using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScreenDeligate : MonoBehaviour
{
    PuzzleScreenController puzzleScreenController;


    // Use this for initialization
    void Start()
    {
        puzzleScreenController = GameObject.Find("Puzzle Screen Controller").GetComponent<PuzzleScreenController>();
    }
    public void ActivateScreen(int id)
    {
        puzzleScreenController.ActivateScreen(id);
    }

    /// <summary>
    /// Deactivates the given screen.
    /// </summary>
    /// <param name="id">The id of the screen.</param>
    public void DeactivateScreen(int id)
    {
        puzzleScreenController.DeactivateScreen(id);
    }

    public void PermenentDeactivate(int id)
    {
        puzzleScreenController.PermenentDeactivate(id);
    }
}
