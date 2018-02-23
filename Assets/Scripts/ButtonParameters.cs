using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonParameters : MonoBehaviour
{

    [SerializeField]
    UnityEvent ContinueEvent;

    [SerializeField]
    Item[] requiredItem;

    [SerializeField]
    bool destroyItemOnUse;

    [SerializeField]
    bool alwaysCheck;

    [SerializeField]
    bool requireInventoryRoom;

    [SerializeField]
    RequiredActivations[] requiredActivations;

    [SerializeField]
    bool useTags;

    [ConditionalHide("useTags", true)]
    public string tagName;


    public void OnClickParamaters()
    {
        if (requiredItem.Length >= 1 && !useTags && GameController.instance.SelectedItem != null)
        {
            for (int i = 0; i < requiredItem.Length; i++)
            {
                if (GameController.instance.SelectedItem == requiredItem[i])
                {
                    if (requireInventoryRoom)
                    {
                        if (GameController.instance.GetCurrentCharacter().IsInventoryFull() == false)
                        {
                            ContinueEvent.Invoke();
                        }
                    }
                    else
                    {
                        ContinueEvent.Invoke();
                    }

                    if (destroyItemOnUse == true)
                    {
                        GameController.instance.GetCurrentCharacter().RemoveItemFromInventory(GameController.instance.SelectedItem);
                        GameController.instance.DeselectItem(GameController.instance.SelectedInventorySlot);
                        GameController.instance.UpdateInventory(GameController.instance.CurrentCharacterID);
                    }
                }
            }
        }
        else if (useTags && GameController.instance.SelectedItem != null)
        {
            if (GameController.instance.SelectedItem.tag == tagName)
            {
                if (requireInventoryRoom)
                {
                    if (GameController.instance.GetCurrentCharacter().IsInventoryFull() == false)
                    {
                        ContinueEvent.Invoke();
                    }
                }
                else
                {
                    ContinueEvent.Invoke();
                }

                if (destroyItemOnUse == true)
                {
                    GameController.instance.GetCurrentCharacter().RemoveItemFromInventory(GameController.instance.SelectedItem);
                    GameController.instance.DeselectItem(GameController.instance.SelectedInventorySlot);
                    GameController.instance.UpdateInventory(GameController.instance.CurrentCharacterID);
                }
            }
        }

        else if (requiredItem.Length <= 0 && !useTags)
        {
            if (requireInventoryRoom)
            {
                if (GameController.instance.GetCurrentCharacter().IsInventoryFull() == false)
                {
                    ContinueEvent.Invoke();
                }
            }
            else
            {
                ContinueEvent.Invoke();
            }
        }
    }

    public void Activate(Button button = null)
    {
        bool allComplete;
        allComplete = Complete(button);
        if (allComplete)
        {
            if (requireInventoryRoom)
            {
                if (GameController.instance.GetCurrentCharacter().IsInventoryFull() == false)
                {
                    ContinueEvent.Invoke();
                }
            }
            else
            {
                ContinueEvent.Invoke();
            }

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
public struct RequiredActivations { public Button requiredButton;[HideInInspector] public bool activated; }
