using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class DialogueController : MonoBehaviour
{
    [Tooltip("Choose text gameobject.")]
    [SerializeField]
    GameObject dialogueTextPrefab;
    [Tooltip("What is the dialogues position relevant to the source.")]
    [SerializeField]
    Vector3 textPosition;
    [SerializeField]
    Vector3 canvasOffset;

    [SerializeField]
    BindSpeakerAndBubble[] bindSpeakerAndBubble;

    [SerializeField]
    Transform puzzleScreenDialogue;

    ConditionsManager conditionsManager;

    List<SpeakerAndText> currentSpeakers = new List<SpeakerAndText>();

    Coroutine currentCoroutineCheck;
    Coroutine currentCoruotineDisplay;


    // Use this for initialization
    void Start()
    {
        conditionsManager = GetComponent<ConditionsManager>();
    }

    public void RunDialogue(EntityValues entityValues)
    {
        if (currentCoroutineCheck != null)
        {
            StopCoroutine(currentCoroutineCheck);
        }
        currentCoroutineCheck = StartCoroutine(CheckConditions(entityValues));
    }

    //Checks all Conditions and returns the values of the first successful
    private IEnumerator CheckConditions(EntityValues entityValues)
    {
        ConditionValues values = conditionsManager.CheckConditions(entityValues);
        if (values.meetComplete == true)
        {
            for (int j = 0; j < conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue.Length; j++)
            {
                if (currentCoruotineDisplay != null)
                {
                    StopCoroutine(currentCoruotineDisplay);
                    currentSpeakers.ForEach(DestroyDialogue);
                }

               currentCoruotineDisplay = StartCoroutine(DisplayDialogue(conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j],
                    conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j].time, values));
                yield return new WaitForSeconds(conditionsManager.container[values.containerNumber].dialogue[values.dialogueNumber].dialogue[j].time);
                print(j.ToString());
            }

        }
    }

    //Displays current dialogue and sets current source
    public IEnumerator DisplayDialogue(DialogueElements dialogue, float time, ConditionValues values)
    {
        conditionsManager.container[values.containerNumber].DialogueRunning = true;
        GameObject dialoguePrefab = dialogueTextPrefab;
        for (int i = 0; i < bindSpeakerAndBubble.Length; i++)
        {
            if (bindSpeakerAndBubble[i].speaker == dialogue.speaker)
            {
                dialoguePrefab = bindSpeakerAndBubble[i].textbubble;
            }
        }
        GameObject textGameobject = Instantiate(dialoguePrefab, new Vector3(0, 0), transform.rotation, dialogue.speaker.transform);
        Text dialogueText = textGameobject.GetComponentInChildren<Text>();
        SpeakerAndText speakerAndText = new SpeakerAndText();
        speakerAndText.currentSpeaker = dialogue.speaker;
        speakerAndText.currentText = textGameobject;
        currentSpeakers.Add(speakerAndText);
        if (GlobalStatics.Language == ELanguage.Swedish)
        {
            dialogueText.text = dialogue.line_Swedish;
            dialogueText.font.material.mainTexture.filterMode = FilterMode.Point;
        }
        if (GlobalStatics.Language == ELanguage.English)
        {
            dialogueText.text = dialogue.line_English;
            dialogueText.font.material.mainTexture.filterMode = FilterMode.Point;
        }

        Animator animator = textGameobject.GetComponentInChildren<Animator>();

        yield return new WaitForSeconds(time);
        animator.Play("BubbleEnd");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
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
        if (GlobalStatics.PuzzleScreenOn == false)
        {
            Camera.main.GetComponent<Camera>().WorldToViewportPoint(speakerAndText.currentSpeaker.transform.position + textPosition);
            speakerAndText.currentText.transform.GetChild(0).transform.position = Camera.main.GetComponent<Camera>().WorldToScreenPoint(speakerAndText.currentSpeaker.transform.position + textPosition);
        }
        else
        {
            if (puzzleScreenDialogue != null)
            {
                speakerAndText.currentText.transform.GetChild(0).transform.position = puzzleScreenDialogue.position + canvasOffset;
            }
        }
    }

    private void DestroyDialogue(SpeakerAndText speakerAndText)
    {
        StartCoroutine(ClosingAnimation(speakerAndText));
    }

    private IEnumerator ClosingAnimation(SpeakerAndText speakerAndText)
    {
        Animator animator = speakerAndText.currentText.GetComponentInChildren<Animator>();
        animator.Play("BubbleEnd");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        currentSpeakers.Remove(speakerAndText);
        Destroy(speakerAndText.currentText);
    }

    // Update is called once per frame
    void Update()
    {
        DialoguePosition();
    }
}

public struct SpeakerAndText
{
    public GameObject currentSpeaker;
    public GameObject currentText;
}

[System.Serializable]
public struct BindSpeakerAndBubble
{
    public GameObject speaker;
    public GameObject textbubble;
}
