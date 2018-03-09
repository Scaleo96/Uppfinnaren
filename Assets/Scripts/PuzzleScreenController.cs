using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleScreenController : MonoBehaviour
{
    GameObject[] puzzleScreens;

    List<int> deactivatedId = new List<int>();

    Vector3 savedPos;
    int savedID;
    float amount = 0.25f;
    float shake = 0;
    float shakeAmount = 0.75f;
    float decreaseAmount = 1;

    private void Awake()
    {
        puzzleScreens = new GameObject[transform.childCount];

        // Puts all the children of the controller into the puzzleScreens array.
        for (int i = 0; i < puzzleScreens.Length; i++)
        {
            puzzleScreens[i] = transform.GetChild(i).gameObject;
        }
    }

    public void PuzzleScreenShake(int id)
    {
        savedPos = puzzleScreens[id].transform.position;
        savedID = id;
        shake = amount;
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

        if (shake > 0)
        {
            puzzleScreens[savedID].transform.position = (Vector2)puzzleScreens[savedID].transform.position + Random.insideUnitCircle * shakeAmount;
            shake -= Time.deltaTime * decreaseAmount;

        }
        else
        {
            puzzleScreens[savedID].transform.position = savedPos;
            shake = 0;
        }
    }
}
