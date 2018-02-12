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
        int i = 0;
        ConditionValues values;
        for (i = 0; i < container.Length;)
        {
            bool test = true;
            for (int j = 0; j < container[i].conditions.Length; j++)
            {
                Inspect(entityValues, container[i].conditions[j]);
                PositionTrigger(entityValues, container[i].conditions[j]);
                PickupItem(entityValues, container[i].conditions[j]);
                EnterDoor(entityValues, container[i].conditions[j]);
                PuzzleSolved(entityValues, container[i].conditions[j]);
                UseItem(entityValues, container[i].conditions[j]);
                if (container[i].conditions[j].activated == false || container[i].isComplete == true)
                {
                    test = false;
                }
            }
            if (container[i].isComplete == false && test && container[i].DialogueRunning == false)
            {
                values.dialogueNumber = container[i].dialogueDone;
                values.meetComplete = true;
                values.containerNumber = i;
                container[i].dialogueDone++;
                if (container[i].dialogueDone >= container[i].dialogue.Length)
                {
                    container[i].isComplete = true;
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

    private void Inspect(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.Inspect && entityValues.trigger == EntityValues.TriggerType.Inspect)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void PositionTrigger(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PositionTrigger && entityValues.trigger == EntityValues.TriggerType.PositionTrigger)
        {
            if (entityValues.collider2d == condition.collisionTrigger && entityValues.character == condition.character)
            {
               condition.activated = true;
            }
        }
    }

    private void PickupItem(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PickupItem && entityValues.trigger == EntityValues.TriggerType.PickupItem)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void EnterDoor(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.EnterDoor && entityValues.trigger == EntityValues.TriggerType.EnterDoor)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void PuzzleSolved(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.PuzzleSolved && entityValues.trigger == EntityValues.TriggerType.PuzzleSolved)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character)
            {
                condition.activated = true;
            }
        }
    }

    private void UseItem(EntityValues entityValues, Conditions condition)
    {
        if (condition.trigger == Conditions.TriggerType.UseItem && entityValues.trigger == EntityValues.TriggerType.UseItem)
        {
            if (entityValues.entity == condition.entity && entityValues.character == condition.character && entityValues.item == condition.item)
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
    public bool isComplete;

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