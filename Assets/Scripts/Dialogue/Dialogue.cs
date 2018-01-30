using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public DialogueElements[] dialogue;
}
[System.Serializable]
public class DialogueElements
{
    public GameObject speaker;
    public string line;
    public float time;
}