using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsManager : MonoBehaviour
{
    [Tooltip("A container contains an unlimited amount of conditions and a dialogue.")]
    public ConditionContainer[] container;

    public ConditionValues CheckConditions(EntityValues entityValues)
    {
        return IsAllComplete(entityValues);
    }

    public ConditionValues IsAllComplete(EntityValues entityValues)
    {
        ConditionValues values;
        for (int i = 0; i < container.Length;)
        {
            // Everything breaks if you remove this
            bool failSafe = true;

            for (int j = 0; j < container[i].conditions.Length; j++)
            {
                if (container[i].DialogueRunning == false)
                {
                    AlreadyUsed(entityValues, container[i].conditions[j]);
                    ContainerEmpty(entityValues, container[i].conditions[j]);
                    FailedUse(entityValues, container[i].conditions[j]);
                    Inspect(entityValues, container[i].conditions[j]);
                    PositionTrigger(entityValues, container[i].conditions[j]);
                    PickupItem(entityValues, container[i].conditions[j]);
                    EnterDoor(entityValues, container[i].conditions[j]);
                    PuzzleSolved(entityValues, container[i].conditions[j]);
                    UseItem(entityValues, container[i].conditions[j]);
                }

                if (container[i].conditions[j].activated == false || container[i].isComplete == true)
                {
                    failSafe = false;
                }
            }
            if (container[i].isComplete == false && failSafe && container[i].DialogueRunning == false)
            {
                values.dialogueNumber = container[i].dialogueDone;
                values.meetComplete = true;
                values.containerNumber = i;
                container[i].dialogueDone++;

                for (int k = 0; k < container[i].conditions.Length; k++)
                {
                    container[i].conditions[k].activated = false;
                }

                if (container[i].dialogueDone >= container[i].dialogue.Length && container[i].repeatable == false)
                {
                    container[i].isComplete = true;
                }
                else if (container[i].dialogueDone >= container[i].dialogue.Length && container[i].repeatable == true)
                {
                    container[i].dialogueDone = 0;
                }
                return values;
            }
            else
            {
                if (i < container.Length)
                {
                    i++;
                }
            }
        }
        values.meetComplete = false;
        values.dialogueNumber = 0;
        values.containerNumber = 0;
        return values;
    }
    private void AlreadyUsed(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.AlreadyUsed && entityValues.trigger == EntityValues.TriggerType.AlreadyUsed)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) : 
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void ContainerEmpty(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.ContainerEmpty && entityValues.trigger == EntityValues.TriggerType.ContainerEmpty)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void FailedUse(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.FailedUse && entityValues.trigger == EntityValues.TriggerType.FailedUse)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void Inspect(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.Inspect && entityValues.trigger == EntityValues.TriggerType.Inspect)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void PositionTrigger(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PositionTrigger && entityValues.trigger == EntityValues.TriggerType.PositionTrigger)
        {
            if (condition.useEntityName ? (entityValues.collider2d == condition.collisionTrigger && entityValues.character.EntityName == condition.characterName) :
                (entityValues.collider2d == condition.collisionTrigger && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void PickupItem(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PickupItem && entityValues.trigger == EntityValues.TriggerType.PickupItem)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void EnterDoor(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.EnterDoor && entityValues.trigger == EntityValues.TriggerType.EnterDoor)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void PuzzleSolved(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PuzzleSolved && entityValues.trigger == EntityValues.TriggerType.PuzzleSolved)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName) :
                (entityValues.entity == condition.entity && entityValues.character == condition.character))
            {
                condition.activated = true;
            }
        }
    }

    private void UseItem(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.UseItem && entityValues.trigger == EntityValues.TriggerType.UseItem)
        {
            if (condition.useEntityName ? (entityValues.entity.EntityName == condition.EntityName && entityValues.character.EntityName == condition.characterName && entityValues.item.EntityName == condition.itemName) : 
                (entityValues.entity == condition.entity && entityValues.character == condition.character && entityValues.item == condition.item))
            {
                condition.activated = true;
            }
        }
    }
}

public struct ConditionValues
{
    public bool meetComplete;
    public int containerNumber;
    public int dialogueNumber;
}

[System.Serializable]
public class ConditionContainer
{
    [Tooltip("What conditions are required to trigger the dialogue.")]
    public Conditions[] conditions;
    [Tooltip("What dialogue is triggered.")]
    public Dialogue[] dialogue;

    [HideInInspector]
    public int dialogueDone;
    [HideInInspector]
    bool dialogueRunning;
    [HideInInspector]
    public bool isComplete;

    public bool repeatable;

    public bool DialogueRunning
    {
        get
        {
            return dialogueRunning;
        }
        set
        {
            dialogueRunning = value;
        }
    }
}