using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public enum Language { Svenska, English }

    [Tooltip("Choose text gameobject.")]
    [SerializeField]
    GameObject dialogueTextPrefab;
    [Tooltip("What is the dialogues position relevant to the source.")]
    [SerializeField]
    Vector3 textPosition;

    public Language language;

    ConditionsManager conditionsManager;

    List<SpeakerAndText> currentSpeakers = new List<SpeakerAndText>();

    Coroutine currentCoroutine;

    // Use this for initialization
    void Start()
    {
        conditionsManager = GetComponent<ConditionsManager>();
    }

    public void RunDialogue(EntityValues entityValues)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(CheckConditions(entityValues));
    }

    //Checks all Conditions and returns the values of the first successful
    private IEnumerator CheckConditions(EntityValues entityValues)
    {
        ConditionValues values = conditionsManager.CheckConditions(entityValues);
        if (values.meetComplete == true)
        {
            for (int j = 0; j < conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue.Length; j++)
            {
                StartCoroutine(DisplayDialogue(conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j],
                    conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j].time, values));
                yield return new WaitForSeconds(conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j].time);
                print(j.ToString());
            }

        }
    }

    //Displays current dialogue and sets current source
    private IEnumerator DisplayDialogue(DialogueElements dialogue, float time, ConditionValues values)
    {
        conditionsManager.container[values.containerNumber].DialogueRunning = true;
        GameObject textGameobject = Instantiate(dialogueTextPrefab, new Vector3(0, 0), transform.rotation, dialogue.speaker.transform);
        Text dialogueText = textGameobject.GetComponentInChildren<Text>();
        //currentText = textGameobject;
        SpeakerAndText speakerAndText = new SpeakerAndText();
        speakerAndText.currentSpeaker = dialogue.speaker;
        speakerAndText.currentText = textGameobject;
        currentSpeakers.Add(speakerAndText);
        //currentSpeaker = dialogue.speaker;
        if (language == Language.Svenska)
        {
            dialogueText.text = dialogue.line_Swedish;
            dialogueText.font.material.mainTexture.filterMode = FilterMode.Point;
        }
        if (language == Language.English)
        {
            dialogueText.text = dialogue.line_English;
            dialogueText.font.material.mainTexture.filterMode = FilterMode.Point;
        }
        yield return new WaitForSeconds(time);
        currentSpeakers.Remove(speakerAndText);
        Destroy(textGameobject);
        conditionsManager.container[values.containerNumber].DialogueRunning = false;
    }

    private void DialoguePosition()
    {
        if (currentSpeakers != null)
        {
            currentSpeakers.ForEach(ChangePosition);
        }
    }

    private void ChangePosition(SpeakerAndText speakerAndText)
    {
        Camera.main.GetComponent<Camera>().WorldToViewportPoint(speakerAndText.currentSpeaker.transform.position + textPosition);
        speakerAndText.currentText.transform.GetChild(0).transform.position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(speakerAndText.currentSpeaker.transform.position + textPosition);
    }

    // Update is called once per frame
    void Update()
    {
        DialoguePosition();
    }
}

public class SpeakerAndText
{
    public GameObject currentSpeaker;
    public GameObject currentText;
}
