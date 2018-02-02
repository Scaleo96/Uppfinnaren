﻿using System.Collections;
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

    public void Call()
    {
        if (call.Method.ContainsGenericParameters)
        {
            Debug.Log("Param exists.");
            call.Invoke((int)call.Method.GetParameters()[0].RawDefaultValue);
        }
        else
        {
            call.Invoke(0);
        }      
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
            new Cheat("SetYPos", delegate { SetYPos(int.Parse(input.Split(' ')[1])); } )
        };

        inputField.onEndEdit.AddListener( delegate { EnterConsole(inputField); } );
    }

    private void Update()
    {
        if (Input.GetKeyDown(activateKey))
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
    }

    private void EnterConsole(InputField inputField)
    {
        input = inputField.text;

        for (int i = 0; i < cheats.Length; i++)
        {
            if (input.ToLower() == cheats[i].GetCode().ToLower())
            {
                cheats[i].Call();
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

    private void SetYPos(float yPos)
    {
        Debug.Log(yPos);

        GameController.instance.GetCurrentCharacter().transform.position = new Vector2
        (
            GameController.instance.GetCurrentCharacter().transform.position.x,
            yPos
        );
    }
}