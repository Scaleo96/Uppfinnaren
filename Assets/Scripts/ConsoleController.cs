using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

class Cheat
{
    public Cheat()
    {

    }

    string code;
    UnityAction<string> call;

    public string GetCode()
    {
        return code;
    }

    public void Call()
    {
        
    }
}

public class ConsoleController : MonoBehaviour
{ // OK Janne...

    [SerializeField]
    InputField inputField;

    Cheat[] cheats =
    {
        new Cheat(),
        
    };

    private void Start()
    {
        inputField.onEndEdit.AddListener( delegate { EnterConsole(inputField); } );
    }

    private void EnterConsole(InputField inputField)
    {
        
    }
}
