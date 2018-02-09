using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonParameters : MonoBehaviour {

    [SerializeField]
    UnityEvent ContinueEvent;

    [SerializeField]
    Item[] requiredItem;

    [SerializeField]
    bool destroyItemOnUse;

    [SerializeField]
    RequiredActivations[] requiredActivations;

    public void OnClickParamaters()
    {
        for (int i = 0; i < requiredItem.Length; i++)
        {
            if (GameController.instance.SelectedItem == requiredItem[i])
            {
                ContinueEvent.Invoke();
                if (destroyItemOnUse == true)
                {
                    GameController.instance.GetCurrentCharacter().RemoveItemFromInventory(GameController.instance.SelectedItem);
                    GameController.instance.DeselectItem(GameController.instance.SelectedInventorySlot);
                    GameController.instance.UpdateInventory(GameController.instance.CurrentCharacterID);
                }
            }
        }
    }

    public void Activate(Button button)
    {
        bool allComplete;
        allComplete = Complete(button);
        if (allComplete)
        {
            ContinueEvent.Invoke();
        }
    }

    private bool Complete(Button button)
    {
        for (int i = 0; i < requiredActivations.Length; i++)
        {
            if (button == requiredActivations[i].requiredButton)
            {
                requiredActivations[i].activated = true;
            }
        }

        for (int i = 0; i < requiredActivations.Length; i++)
        {
            if (requiredActivations[i].activated == false)
            {
                return false;
            }

        }
        return true;
    }
}

[System.Serializable]
public struct RequiredActivations{ public Button requiredButton; [HideInInspector] public bool activated; }
