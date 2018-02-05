using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField]
    float cutsceneDuration;

    private void Start()
    {
        StartCoroutine(SetActiveAfterTime(false, cutsceneDuration));
    }

    IEnumerator SetActiveAfterTime(bool active, float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(active);
    }
}
