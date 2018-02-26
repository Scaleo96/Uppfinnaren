using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


// Courtesy of the Lead Distractor (Gif Master)
class Cheat
{
    string code;
    UnityAction<int> call;

    public Cheat(string code, UnityAction<int> call)
    {
        this.code = code;
        this.call = call;
    }

    public string GetCode()
    {
        return code;
    }

    public void Call(int param = 0)
    {
        call.Invoke(param);   
    }
}

[RequireComponent(typeof(Animator))]
public class ConsoleController : MonoBehaviour
{ // OK Janne...

    [SerializeField]
    InputField inputField;
    string input;

    [SerializeField]
    KeyCode activateKey;


    bool isActive;
    Animator animator; 

    [Header("Cheat Resources")]
    [SerializeField]
    GameObject IvarPrefab;
    
    Cheat[] cheats;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cheats = new Cheat[]
        {
            new Cheat("IvarStrike", delegate { IvarStrike(); } ),
            new Cheat("SetYPos", delegate { SetYPos(); } ),
            new Cheat("Godzilla", delegate { ScaleUpCurrentPlayer(); }),
            new Cheat("Gnome", delegate { ScaleDownCurrentPlayer(); }),
            new Cheat("IvarHeaven", delegate{ ChangeAllSpritesToIvar(); })
        };

        inputField.onEndEdit.AddListener( delegate { EnterConsole(inputField); } );
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey))
        {
            ToggleConsole();
        }
    }

    private void ToggleConsole()
    {
        if (isActive)
        {
            animator.SetTrigger("up");
        }
        else
        {
            animator.SetTrigger("down");
            inputField.Select();
        }

        isActive = !isActive;
    }

    private void EnterConsole(InputField inputField)
    {
        input = inputField.text;

        for (int i = 0; i < cheats.Length; i++)
        {
            if (input.Split(' ')[0].ToLower() == cheats[i].GetCode().ToLower())
            {
                if (input.Split(' ').Length > 1)
                {
                    cheats[i].Call(int.Parse(input.Split(' ')[1]));
                }
                else
                {
                    cheats[i].Call();
                }

                inputField.text = "";
            }
        }
    }

    private void IvarStrike()
    {
        int ivarCount = 20;

        Vector2 charPos = GameController.instance.GetCurrentCharacter().transform.position;

        float xPosMin = charPos.x - 5f;
        float xPosMax = charPos.x + 5f;

        float yPosMin = 50f;
        float yPosMax = 70f;

        for (int i = 0; i < ivarCount; i++)
        {
            Vector2 pos = new Vector2
            (
                Random.Range(xPosMin, xPosMax),
                Random.Range(yPosMin, yPosMax)
            );

            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            Instantiate(IvarPrefab, pos, rotation);
            
        }
    }

    private void SetYPos(int yPos = 10)
    {
        Debug.Log(yPos != 10 ? "it worked!!!!!" : "10");

        GameController.instance.GetCurrentCharacter().transform.position = new Vector2
        (
            GameController.instance.GetCurrentCharacter().transform.position.x,
            yPos
        );
    }

    private void ScaleUpCurrentPlayer()
    {
        GameController.instance.GetCurrentCharacter().transform.localScale += Vector3.one * 3;
    }

    private void ScaleDownCurrentPlayer()
    {
        GameController.instance.GetCurrentCharacter().transform.localScale -= Vector3.one * 3;
    }

    private void ChangeAllSpritesToIvar()
    {
        StartCoroutine(ChangeAllSpritesWithDelay(new Vector2(0f, 0.1f), IvarPrefab.GetComponent<SpriteRenderer>().sprite));
    }

    private IEnumerator ChangeAllSpritesWithDelay(Vector2 waitTimeInterval, Sprite sprite)
    {
        foreach (SpriteRenderer spriteRenderer in FindObjectsOfType<SpriteRenderer>())
        {
            yield return new WaitForSeconds(Random.Range(waitTimeInterval.x, waitTimeInterval.y));
            spriteRenderer.sprite = sprite;
        }
    }
}
