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
    bool alwaysCheck;

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

    public void Activate(Button button = null)
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

    private void Update()
    {
        if (alwaysCheck)
        {
            CheckButtons();
        }
    }

    private void CheckButtons()
    {
        for (int i = 0; i < requiredActivations.Length; i++)
        {
            if (requiredActivations[i].requiredButton.gameObject.activeInHierarchy)
            {
                requiredActivations[i].activated = true;
                Activate();
            }
            else
            {
                requiredActivations[i].activated = false;
            }
        }
    }
}

[System.Serializable]
public struct RequiredActivations{ public Button requiredButton; [HideInInspector] public bool activated; }
