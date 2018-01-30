using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Tooltip("Choose text gameobject.")]
    [SerializeField]
    Text dialogueText;
    [Tooltip("What is the dialogues position relevant to the source.")]
    [SerializeField]
    Vector3 textPosition;

    ConditionsManager conditionsManager;
    GameObject speaker;

    // Use this for initialization
    void Start()
    {
        conditionsManager = GetComponent<ConditionsManager>();
    }

    public void RunDialogue(EntityValues entityValues)
    {
        StartCoroutine(CheckConditions(entityValues));
    }

    //Checks all Conditions and returns the values of the first successful
    private IEnumerator CheckConditions(EntityValues entityValues)
    {
        ConditionValues values = conditionsManager.CheckConditions(entityValues);
        if (values.meetComplete == true)
        {
            for (int j = 0; j < conditionsManager.container[values.containerNumber].dialogue.dialogue.Length; j++)
            {
                DisplayDialogue(conditionsManager.container[values.containerNumber].dialogue.dialogue[j]);
                yield return new WaitForSeconds(conditionsManager.container[values.containerNumber].dialogue.dialogue[j].time);
                dialogueText.GetComponentInChildren<SpriteRenderer>().enabled = false;
                dialogueText.text = "";
                print(j.ToString());
            }

        }
    }

    //Displays current dialogue and sets current source
    private void DisplayDialogue(DialogueElements dialogue)
    {
        dialogueText.GetComponentInChildren<SpriteRenderer>().enabled = true;
        dialogueText.text = dialogue.line;
        speaker = dialogue.speaker;
    }

    //Sets position on the source
    private void DialoguePosition()
    {
        if (speaker != null)
        {
            dialogueText.transform.position = speaker.transform.position + textPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DialoguePosition();
    }
}
