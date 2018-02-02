﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public enum Language { Svenska, English}

    [Tooltip("Choose text gameobject.")]
    [SerializeField]
    GameObject dialogueTextPrefab;
    [Tooltip("What is the dialogues position relevant to the source.")]
    [SerializeField]
    Vector3 textPosition;

    public Language language;

    ConditionsManager conditionsManager;
    GameObject currentSpeaker;
    Text currentText;

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
                StartCoroutine( DisplayDialogue(conditionsManager.container[values.containerNumber].dialogue.dialogue[j],
                    conditionsManager.container[values.containerNumber].dialogue.dialogue[j].time) );
                yield return new WaitForSeconds(conditionsManager.container[values.containerNumber].dialogue.dialogue[j].time);
                print(j.ToString());
            }

        }
    }

    //Displays current dialogue and sets current source
    private IEnumerator DisplayDialogue(DialogueElements dialogue, float time)
    {
        GameObject textGameobject = Instantiate(dialogueTextPrefab, dialogue.speaker.transform.position, transform.rotation, dialogue.speaker.transform);
        Text dialogueText = textGameobject.GetComponentInChildren<Text>();
        textGameobject.GetComponent<Canvas>().worldCamera = Camera.main.GetComponent<Camera>();
        currentText = dialogueText;
        currentSpeaker = dialogue.speaker;
        if(language == Language.Svenska)
        {
            dialogueText.text = dialogue.line_Swedish;
        }
        if (language == Language.English)
        {
            dialogueText.text = dialogue.line_English;
        }
        //textGameobject.transform.position = dialogue.speaker.transform.position + textPosition;
        yield return new WaitForSeconds(time);
        Destroy(textGameobject);
    }

    private void DialoguePosition()
    {
        if (currentSpeaker != null && currentText != null)
        {
            currentText.gameObject.transform.position = currentSpeaker.transform.position + textPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DialoguePosition();
    }
}