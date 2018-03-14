using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonParameters : MonoBehaviour
{

    [SerializeField]
    ValueEvent ContinueEvent;

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

    public UIMovable[] requiredUIMovable;

    [SerializeField]
    bool useTags;

    [ConditionalHide("useTags", true)]
    public string tagName;

    [Tooltip("Set this to the entity used to start the puzzle if you want to start dialogue")]
    [SerializeField]
    private Entity triggerEntity;

    private EntityValues values;


    private void Start()
    {
        values.entity = triggerEntity;
        values.collider2d = null;
        values.character = GameController.instance.GetCurrentCharacter();
        values.item = null;
        values.trigger = EntityValues.TriggerType.PuzzleSolved;
    }

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
                            ContinueEvent.Invoke(values);
                        }
                    }
                    else
                    {
                        ContinueEvent.Invoke(values);
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
                        ContinueEvent.Invoke(values);
                    }
                }
                else
                {
                    ContinueEvent.Invoke(values);
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
                    ContinueEvent.Invoke(values);
                }
            }
            else
            {
                ContinueEvent.Invoke(values);
            }
        }
    }

    public void Activate(GameObject button = null)
    {
        bool allComplete;
        allComplete = Complete(button);
        if (allComplete)
        {
            if (requireInventoryRoom)
            {
                if (GameController.instance.GetCurrentCharacter().IsInventoryFull() == false)
                {
                    ContinueEvent.Invoke(values);
                }
            }
            else
            {
                ContinueEvent.Invoke(values);
            }

        }
    }

    private bool Complete(GameObject button)
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
            CheckUI();
        }
    }

    private void CheckButtons()
    {
        if (requiredActivations.Length > 0)
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

    private bool CheckUI()
    {
        if (requiredUIMovable.Length > 0)
        {
            for (int i = 0; i < requiredUIMovable.Length; i++)
            {
                if (requiredUIMovable[i].activated == false)
                {
                    return false;
                }
            }
            ContinueEvent.Invoke(values);
        }

        return true;
    }
}

[System.Serializable]
public struct RequiredActivations { public GameObject requiredButton;[HideInInspector] public bool activated; }
