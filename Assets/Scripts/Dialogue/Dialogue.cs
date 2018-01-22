using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject {
    public DialogueElements[] dialogue;
}
[System.Serializable]
public class DialogueElements
{
   public Entity speaker;
   public string line;
}