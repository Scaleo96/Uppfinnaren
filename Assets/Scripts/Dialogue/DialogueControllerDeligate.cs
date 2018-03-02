using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueControllerDeligate : MonoBehaviour
{

    DialogueController dialogueController;

    // Use this for initialization
    void Start()
    {
        dialogueController = GameObject.Find("DialogueManager").GetComponent<DialogueController>();
    }
    public void RunDialogue(EntityValues entityValues)
    {
        dialogueController.RunDialogue(entityValues);
    }
}
